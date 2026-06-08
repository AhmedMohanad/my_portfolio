// Services/ImageServices/ImageServices.cs
using Microsoft.EntityFrameworkCore;
using Portofolio.Data;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.LoggingServices;
using Portofolio.Models.ImageModel;

namespace Portofolio.Services.ImageServices
{
    public class ImageServices : IImageServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsFolder = "uploads/images";
        private readonly ISimpleLogger _logger;

        public ImageServices(ApplicationDbContext context, IWebHostEnvironment environment, ISimpleLogger logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ImageData> UploadProfileImageAsync(UploadImgDTO dto)
        {
            // Make sure the profile exists first
            var profile = await _context.Profiles.FindAsync(dto.ProfileId);
            if (profile == null)
            {
                _logger.LogWarning("Profile not found for image upload: " + dto.ProfileId );
                throw new KeyNotFoundException("Profile not found.");
            }
                

            string imageUrl;

            if (dto.File != null)
            {
                // Save the file to wwwroot/images/
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                Directory.CreateDirectory(uploadsFolder); // creates folder if it doesn't exist

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dto.File.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.File.CopyToAsync(stream);

                imageUrl = $"/images/{fileName}";
            }
            else if (!string.IsNullOrEmpty(dto.ImageUrl))
            {
                // Just store the external URL directly (GitHub avatar, etc.)
                imageUrl = dto.ImageUrl;
            }
            else
            {
                _logger.LogError("File is null for profile ID: " + dto.ProfileId);
                throw new InvalidOperationException("Either a file or an image URL must be provided.");
            }

            var image = new ImageData
            {
                Url = imageUrl,
                ProfileId = dto.ProfileId,
                UploadedAt = DateTime.UtcNow
            };

            _context.ImageDatas.Add(image);
            await _context.SaveChangesAsync();
            _logger.LogInfo("Image uploaded successfully for profile ID: " + dto.ProfileId);

            return image;
        }



        public async Task<List<ResponseImageDTO>> GetUserImagesOrderedByDateAsync(int profileId)
        {
            _logger.LogInfo("Retrieving images for profile ID: " + profileId);
            var images = await _context.ImageDatas
                .Include(i => i.Profile)
                .Where(i => i.ProfileId == profileId)
                .OrderByDescending(i => i.UploadedAt)
                .Select(i => new ResponseImageDTO
                {
                    Id = i.Id,
                    Url = i.Url,
                    UploadedAt = i.UploadedAt,
                    ProfileId = i.ProfileId,
                    ProfileFullName = i.Profile != null ? i.Profile.FullName : null  
                })
                .ToListAsync();
            
            return images;
        }

        public async Task<bool> DeleteProfileImageAsync(int id)
        {
            _logger.LogInfo("Attempting to delete image with ID: " + id);
            var image = await _context.ImageDatas.FindAsync(id);
            if (image == null)
                return false;

            // only delete physical file if it's a local file (not an external URL)
            if (!image.Url.StartsWith("http://") && !image.Url.StartsWith("https://"))
            {
                var filePath = Path.Combine(_environment.WebRootPath, image.Url.TrimStart('/'));
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }

            _context.ImageDatas.Remove(image);
            await _context.SaveChangesAsync();
            return true;
        }

        

       
    }
}