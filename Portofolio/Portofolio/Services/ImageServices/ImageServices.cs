using Microsoft.EntityFrameworkCore;
using Portofolio.Data;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.Models.ImageModel;

namespace Portofolio.Services.ImageServices
{
    public class ImageServices   : IImageServices
    {

        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly string _uploadsFolder = "uploads/images";
        public ImageServices(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<ImageResponseDTO> UploadImageAsync(UploadImgDTO createDto)
        {
            // check profile 
            var profile = await _context.Profiles.FindAsync(createDto.ProfileId); // has to edit for get profile id
            if (profile == null)
                throw new ArgumentException($"Profile with ID {createDto.ProfileId} not found");

            string? savedUrl = null;

            if (createDto.File != null && createDto.File.Length > 0)
            {
                // upload new file
                savedUrl = await SaveImageFileAsync(createDto.File, createDto.ProfileId);
            }

            else
            {
                throw new ArgumentException("Either image or ImageUrl must be provided");
            }

            // create image
            var imageData = new ImageData
            {
                Url = savedUrl,
                UploadedAt = DateTime.Now,
                ProfileId = createDto.ProfileId
            };

            _context.ImageDatas.Add(imageData);
            await _context.SaveChangesAsync();

            return new ImageResponseDTO
            {
                Id = imageData.Id,
                Url = imageData.Url,
                UploadedAt = imageData.UploadedAt,
                ProfileId = imageData.ProfileId,
                FullName = profile.FullName
            };
        }

        public async Task<ImageResponseDTO?> GetImageByIdAsync(int id)
        {
            var image = await _context.ImageDatas
                .Include(i => i.Profile)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
                return null;

            return new ImageResponseDTO
            {
                Id = image.Id,
                Url = image.Url,
                UploadedAt = image.UploadedAt,
                ProfileId = image.ProfileId,
                FullName = image.Profile?.FullName
            };
        }

        public async Task<List<ImageResponseDTO>> GetAllImagesOrderedByDateAsync()
        {
            var images = await _context.ImageDatas
                .Include(i => i.Profile)
                .OrderByDescending(i => i.UploadedAt)  // New to old
                .Select(i => new ImageResponseDTO
                {
                    Id = i.Id,
                    Url = i.Url,
                    UploadedAt = i.UploadedAt,
                    ProfileId = i.ProfileId,
                    FullName = i.Profile != null ? i.Profile.FullName : null
                })
                .ToListAsync();

            return images;
        }

        private async Task<string> SaveImageFileAsync(IFormFile file, int profileId)
        {
            // check file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only image files are allowed (jpg, jpeg, png, gif, webp, bmp)");

            // check the image size
            if (file.Length > 10 * 1024 * 1024)
                throw new ArgumentException("File size cannot exceed 10 MB");

            // create uniqe name
            var fileName = $"img_{profileId}_{DateTime.Now:yyyyMMdd_HHmmss_fff}{extension}";

            // check for upload folder
            var uploadPath = Path.Combine(_environment.WebRootPath, _uploadsFolder);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            // save the file
            var filePath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // return the relative URL to be stored in the database
            return $"/{_uploadsFolder}/{fileName}";
        }


        public async Task<bool> DeleteImageAsync(int id)
        {
            var image = await _context.ImageDatas.FindAsync(id);
            if (image == null)
                return false;
            // delete the file from the server
            var filePath = Path.Combine(_environment.WebRootPath, image.Url.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);
            // remove from database
            _context.ImageDatas.Remove(image);
            await _context.SaveChangesAsync();
            return true;

        }

    }
}
