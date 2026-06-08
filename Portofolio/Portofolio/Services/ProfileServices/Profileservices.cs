// Services/ProfileServices/ProfileServices.cs
using Portofolio.Data;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.DTOs.ProfileDTOs;
using Portofolio.DTOs.ExperienceDTOs;
using Portofolio.Models.UserModels;
using Portofolio.Services.ImageServices;
using Microsoft.EntityFrameworkCore;

namespace Portofolio.Services.ProfileServices
{
    public class ProfileServices : IProfileServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IImageServices _imageServices;
        private readonly ILogger<ProfileServices> _logger;

        public ProfileServices(ApplicationDbContext context, IImageServices imageServices, ILogger<ProfileServices> logger)
        {
            _context = context;
            _imageServices = imageServices;
            _logger = logger;
        }

       

        public async Task<ResponseProfileDTO> CreateProfileAsync(CreateProfileDTO createDto)
        {
            if (string.IsNullOrWhiteSpace(createDto.FullName) ||
                string.IsNullOrWhiteSpace(createDto.Email) ||
                string.IsNullOrWhiteSpace(createDto.Password))
            {
                _logger.LogWarning("Attempt to create profile with missing required fields: {FullName}, {Email}", createDto.FullName, createDto.Email);
                throw new ArgumentException("FullName, Email, and Password are required");
            }
            // Check if email already exists
            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Email == createDto.Email);

            if (existingProfile != null)
            {
                _logger.LogWarning("Attempt to create profile with existing email: {Email}", createDto.Email);
                throw new InvalidOperationException("Email already exists");

            }
               

            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

            var profile = new Profile
            {
                FullName = createDto.FullName,
                Email = createDto.Email,
                PasswordHash = passwordHash,
                PhoneNumber = createDto.PhoneNumber,
                Bio = createDto.Bio ?? string.Empty
            };

            _context.Profiles.Add(profile);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Profile created with ID: {ProfileId} and Email: {Email}", profile.Id, profile.Email);

            //  Profile picture is uploaded separately after creation via /api/images


            return new ResponseProfileDTO
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Bio = profile.Bio,
                ProfilePictureUrl = null,   // uploaded separately
                Experiences = new()
            };
        }

        public async Task<ResponseProfileDTO?> GetProfileByIdAsync(int id)
        {
            var profile = await _context.Profiles
                .Include(p => p.Experiences)
                .Include(p => p.ProfilePictures)    
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
                return null;

            //  use [NotMapped] computed property from the model
            var latestImage = profile.ProfilePicture;

            return new ResponseProfileDTO
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Bio = profile.Bio,
                ProfilePictureUrl = latestImage?.Url,
                Experiences = profile.Experiences.Select(e => new ResponseExperienceDTO
                {  // it hase to replace with methid from experience services but for now it is here
                    Id = e.Id,                      
                    Company = e.Company,
                    Role = e.Role,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    ProfileName = profile.FullName  
                }).ToList()
            };
        }
    }
}