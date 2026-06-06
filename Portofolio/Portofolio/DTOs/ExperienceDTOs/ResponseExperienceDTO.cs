// DTOs/ExperienceDTOs/ResponseExperienceDTO.cs
namespace Portofolio.DTOs.ExperienceDTOs
{
    public class ResponseExperienceDTO
    {
        public int Id { get; set; }  
        public string Company { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }  // null = present
        public bool IsCurrent => EndDate == null; 
        public string? ProfileName { get; set; }
    }
}