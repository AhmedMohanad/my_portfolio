// DTOs/ExperienceDTOs/CreateExperienceDTO.cs
using System.ComponentModel.DataAnnotations;

namespace Portofolio.DTOs.ExperienceDTOs
{
    public class CreateExperienceDTO
    {
        [Required]
        [MaxLength(200)]
        public string Company { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Role { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }  // null = present

        [Required]
        public int ProfileId { get; set; }
    }
}