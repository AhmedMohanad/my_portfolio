using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portofolio.Data;
using Portofolio.DTOs.AuthDTOs;
using Portofolio.Models;
using Portofolio.Models.UserModels;
using Portofolio.Services.JWTServices;
using Portofolio.Services.ProfileServices;


namespace Portofolio.Services.AuthServices
{
    public class AuthServices : IAuthServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IProfileServices _profileServices;
        private readonly IJwtServices _jwtServices;


        public AuthServices(ApplicationDbContext context, IProfileServices ps, IJwtServices jwtServices)
        {
            _context = context;
            _profileServices = ps;
            _jwtServices = jwtServices;
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

       
    }
}