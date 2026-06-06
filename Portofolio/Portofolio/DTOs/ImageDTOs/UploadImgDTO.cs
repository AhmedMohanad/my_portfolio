// DTOs/ImageDTOs/UploadImgDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Portofolio.DTOs.ImageDTOs
{
    public class UploadImgDTO
    {
        [Required]
        public int ProfileId { get; set; }

        
        public IFormFile? File { get; set; }    // for direct file upload
        public string? ImageUrl { get; set; }   // for external URL (e.g. GitHub avatar)
    }
}