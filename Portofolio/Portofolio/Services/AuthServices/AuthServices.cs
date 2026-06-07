using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portofolio.Data;
using Portofolio.DTOs.AuthDTOs;
using Portofolio.Models;
using Portofolio.Models.UserModels;
using Portofolio.Services.JWTServices;
using Portofolio.Services.ProfileServices;
using System.IdentityModel.Tokens.Jwt;


namespace Portofolio.Services.AuthServices
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDbContext _context;
        
        private readonly IJwtServices _jwtServices;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AuthServices(ApplicationDbContext context,IJwtServices jwtServices, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
           
            _jwtServices = jwtServices;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string?> RegisterAsync(RegisterDTO dto)
        {
            bool emailExists = await _context.Profiles.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                throw new InvalidOperationException("Email already in use.");

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

            return _jwtServices.GenerateJwtToken(user);
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {

            //  check if user is already logged in by checking for existing token in the request header and verifying if it's blacklisted
            var currentToken = _httpContextAccessor.HttpContext?.Request
                .Headers["Authorization"].ToString().Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(currentToken))
            {
                bool isBlacklisted = await _context.BlacklistedTokens
                    .AnyAsync(t => t.Token == currentToken);

                if (!isBlacklisted)
                    throw new InvalidOperationException("You are already logged in.");
            }

            // Find user by email
            var user = await _context.Profiles.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                throw new KeyNotFoundException("Invalid email or password.");

            // Verify password
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid email or password.");

            return _jwtServices.GenerateJwtToken(user);
        }


        public async Task LogoutAsync(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            _context.BlacklistedTokens.Add(new BlacklistedToken
            {
                Token = token,
                ExpiresAt = jwt.ValidTo
            });

            await _context.SaveChangesAsync();
        }


    }
}