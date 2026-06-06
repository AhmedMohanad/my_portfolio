// DTOs/ProfileDTOs/CreateProfileDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Portofolio.DTOs.ProfileDTOs
{
    public class CreateProfileDTO
    {
        [Required]
        [MaxLength(75)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Bio { get; set; } = string.Empty;
       
    }
}