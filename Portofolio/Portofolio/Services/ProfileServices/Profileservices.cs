using Portofolio.Data;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.DTOs.ProfileDTOs;
using Portofolio.Models.UserModels;
using Portofolio.Services.ImageServices;
using Microsoft.EntityFrameworkCore;


namespace Portofolio.Services.ProfileServices
{
    public class Profileservices : IProfileServices
    {

        private readonly ApplicationDbContext _context;
        private readonly IImageServices _imageServices;

        public Profileservices(ApplicationDbContext portofolioDbContext, IImageServices imageServices)
        {
            _context = portofolioDbContext;
            _imageServices = imageServices;
        }

        // Create a new profile
        public async Task<ResponseProfileDTO> CreateProfileAsync(CreateProfileDTO createDto)
        {
            // Check if email already exists
            var existingProfile = await _context.Profiles
                                .FirstOrDefaultAsync(p => p.Email == createDto.Email);

            if (existingProfile != null)
                throw new InvalidOperationException("Email already exists");

            // Hash the password
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(createDto.Password);

            // Create new profile
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

            // Upload profile picture if provided
            string? photoUrl = null;
            if (createDto.ProfilePicture != null && createDto.ProfilePicture.Length > 0)
            {
                try
                {
                    var uploadDto = new UploadImgDTO
                    {
                        File = createDto.ProfilePicture,
                        ProfileId = profile.Id
                    };
                    var uploadedImage = await _imageServices.UploadImageAsync(uploadDto);
                    photoUrl = uploadedImage.Url;
                }
                catch (Exception ex)
                {
                    // Log error but don't fail profile creation
                    Console.WriteLine($"Failed to upload profile picture: {ex.Message}");
                }
            }

            return new ResponseProfileDTO
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Bio = profile.Bio,
                ProfilePictureUrl = photoUrl,
                CreatedAt = DateTime.Now
            };
        }


        // Get profile by ID with all related data
        public async Task<ResponseProfileDTO?> GetProfileByIdAsync(int id)
        {
            var profile = await _context.Profiles
                .Include(p => p.Experiences)
                .Include(p => p.ProfilePictureUrl)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (profile == null)
                return null;

            // Get latest profile picture
            var latestImage = profile.ProfilePictureUrl
                .OrderByDescending(i => i.UploadedAt)
                .FirstOrDefault();

            return new ResponseProfileDTO
            {
                Id = profile.Id,
                FullName = profile.FullName,
                Email = profile.Email,
                PhoneNumber = profile.PhoneNumber,
                Bio = profile.Bio,
                ProfilePictureUrl = latestImage?.Url,
                Experiences = profile.Experiences.Select(e => new ExperienceDTO
                {
                    Id = e.Id,
                    Company = e.Company,
                    Role = e.Role,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                }).ToList(),
                CreatedAt = DateTime.Now
            };
        }




    }
}
