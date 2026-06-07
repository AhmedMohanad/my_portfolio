using System.ComponentModel.DataAnnotations;

namespace Portofolio.DTOs.ExperienceDTOs
{
    public class UpdateExperienceDTO
    {

        
        [MaxLength(200)]
        public string? Company { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Role { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

       
        public DateTime? StartDate { get; set; } 

        public DateTime? EndDate { get; set; }  // null = present

        
        public int ProfileId { get; set; }
    }
}
