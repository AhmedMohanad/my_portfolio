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

        public ProfileServices(ApplicationDbContext context, IImageServices imageServices)
        {
            _context = context;
            _imageServices = imageServices;
        }

        public async Task<ResponseProfileDTO> CreateProfileAsync(CreateProfileDTO createDto)
        {
            // Check if email already exists
            var existingProfile = await _context.Profiles
                .FirstOrDefaultAsync(p => p.Email == createDto.Email);

            if (existingProfile != null)
                throw new InvalidOperationException("Email already exists");

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
                {
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