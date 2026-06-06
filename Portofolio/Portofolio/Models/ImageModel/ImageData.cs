// Models/ImageModel/ImageData.cs
using Portofolio.Models.UserModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portofolio.Models.ImageModel
{
    public class ImageData
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(500)]
        public string Url { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;  // UTC, not local

        // Foreign key
        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;
    }
}