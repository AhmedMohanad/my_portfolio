// DTOs/ImageDTOs/ResponseImageDTO.cs
namespace Portofolio.DTOs.ImageDTOs
{
    public class ResponseImageDTO
    {
        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int ProfileId { get; set; }
        public string? ProfileFullName { get; set; }  
    }
}