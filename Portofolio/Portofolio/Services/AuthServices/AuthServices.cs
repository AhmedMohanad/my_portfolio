using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portofolio.Data;
using Portofolio.DTOs.AuthDTOs;
using Portofolio.LoggingServices;
using Portofolio.Models;
using Portofolio.Models.UserModels;
using Portofolio.Services.JWTServices;
using Portofolio.Services.ProfileServices;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace Portofolio.Services.AuthServices
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDbContext _context;
        
        private readonly IJwtServices _jwtServices;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISimpleLogger _logger;


        public AuthServices(ApplicationDbContext context,IJwtServices jwtServices, IHttpContextAccessor httpContextAccessor, ISimpleLogger logger)
        {
            _context = context;
           
            _jwtServices = jwtServices;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<string?> RegisterAsync(RegisterDTO dto)
        {
            _logger.LogInfo($"Login attempt for: {dto.Email}");
            bool emailExists = await _context.Profiles.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
            {
                _logger.LogWarning($"Email already in use: {dto.Email}");
                throw new InvalidOperationException("Email already in use.");
            }
                

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var user = new Profile
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = dto.PhoneNumber,
                Bio = string.Empty,

            };

            _context.Profiles.Add(user);
            await _context.SaveChangesAsync();
            _logger.LogSuccess($"User registered successfully: {user.Email}");
            return _jwtServices.GenerateJwtToken(user);
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            _logger.LogInfo($"Login attempt for: {dto.Email}");
            //  check if user is already logged in by checking for existing token in the request header and verifying if it's blacklisted
            var currentToken = _httpContextAccessor.HttpContext?.Request
                .Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(currentToken))
            {
                bool isBlacklisted = await _context.BlacklistedTokens
                    .AnyAsync(t => t.Token == currentToken);

                if (!isBlacklisted)
                {
                    _logger.LogWarning($"Login attempt while already logged in: {dto.Email}");
                    throw new InvalidOperationException("You are already logged in.");
                }
                   
            }

            // Find user by email
            var user = await _context.Profiles.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                _logger.LogWarning($"Login failed no email found: {dto.Email}");
                throw new KeyNotFoundException("Invalid email or password.");
            }

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                _logger.LogWarning($"Login failed invalid password: {dto.Email}");
                throw new UnauthorizedAccessException("Invalid email or password.");
            }

            _logger.LogSuccess($"User logged in successfully: {user.Email}");
            return _jwtServices.GenerateJwtToken(user);
        }


        public async Task LogoutAsync(string token)
        {
           
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // Extract email from token claims
            var email = jwt.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value
                        ?? jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value
                        ?? jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            _logger.LogInfo($"User {email} is logging out. Token expires at: {jwt.ValidTo}");

            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                Token = token,
                ExpiresAt = jwt.ValidTo
            });

            await _context.SaveChangesAsync();

            _logger.LogInfo($"User {email} successfully logged out and token blacklisted");
        }

    }
}