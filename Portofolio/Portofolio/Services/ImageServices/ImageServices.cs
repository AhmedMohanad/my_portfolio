// Services/ImageServices/ImageServices.cs
using Microsoft.EntityFrameworkCore;
using Portofolio.Data;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.Models.ImageModel;

namespace Portofolio.Services.ImageServices
{
    public class ImageServices : IImageServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsFolder = "uploads/images";

        public ImageServices(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ResponseImageDTO> UploadImageAsync(UploadImgDTO createDto)
        {
            var profile = await _context.Profiles.FindAsync(createDto.ProfileId);
            if (profile == null)
                throw new ArgumentException($"Profile with ID {createDto.ProfileId} not found");

            // ✅ handle both upload strategies
            string savedUrl;

            if (createDto.File != null && createDto.File.Length > 0)
            {
                savedUrl = await SaveImageFileAsync(createDto.File, createDto.ProfileId);
            }
            else if (!string.IsNullOrWhiteSpace(createDto.ImageUrl))
            {
                savedUrl = createDto.ImageUrl;  // store external URL directly
            }
            else
            {
                throw new ArgumentException("Either a file or an ImageUrl must be provided");
            }

            var imageData = new ImageData
            {
                Url = savedUrl,
                UploadedAt = DateTime.UtcNow,   
                ProfileId = createDto.ProfileId
            };

            _context.ImageDatas.Add(imageData);
            await _context.SaveChangesAsync();

            return new ResponseImageDTO
            {
                Id = imageData.Id,
                Url = imageData.Url,
                UploadedAt = imageData.UploadedAt,
                ProfileId = imageData.ProfileId,
                ProfileFullName = profile.FullName 
            };
        }

        public async Task<ResponseImageDTO?> GetImageByIdAsync(int id)
        {
            var image = await _context.ImageDatas
                .Include(i => i.Profile)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
                return null;

            return new ResponseImageDTO
            {
                Id = image.Id,
                Url = image.Url,
                UploadedAt = image.UploadedAt,
                ProfileId = image.ProfileId,
                ProfileFullName = image.Profile?.FullName   
            };
        }

        public async Task<List<ResponseImageDTO>> GetUserImagesOrderedByDateAsync(int profileId)
        {
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

        public async Task<bool> DeleteImageAsync(int id)
        {
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

        private async Task<string> SaveImageFileAsync(IFormFile file, int profileId)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only image files are allowed (jpg, jpeg, png, gif, webp, bmp)");

            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 10 MB");

            //  use UTC for consistent file naming
            var fileName = $"img_{profileId}_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}{extension}";

            var uploadPath = Path.Combine(_environment.WebRootPath, _uploadsFolder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{_uploadsFolder}/{fileName}";
        }
    }
}