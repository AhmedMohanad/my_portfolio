// Models/UserModels/Profile.cs
using Portofolio.Models.ImageModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portofolio.Models.UserModels
{
    public class Profile
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(75)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Bio { get; set; } = string.Empty;

        // Navigation properties
        public List<ImageData> ProfilePictures { get; set; } = new();
        public List<Experience> Experiences { get; set; } = new();

        // Computed — NOT stored in DB
        [NotMapped]
        public ImageData? ProfilePicture => ProfilePictures.MaxBy(i => i.UploadedAt);

        [NotMapped]
        public Experience? CurrentExperience => Experiences.FirstOrDefault(e => e.EndDate == null);
    }
}