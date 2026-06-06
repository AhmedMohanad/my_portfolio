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
        public string PhoneNumber { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;

        // relationships
        public List<ImageData> ProfilePictureUrl { get; set; } = new List<ImageData>();
        public List<Experience> Experiences { get; set; } = new List<Experience>();








    }
}
