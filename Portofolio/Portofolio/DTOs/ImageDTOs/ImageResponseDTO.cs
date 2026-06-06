namespace Portofolio.DTOs.ImageDTOs
{
    public class ImageResponseDTO
    {

        public int Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
        public int ProfileId { get; set; }
        public string? FullName { get; set; }
    }
}
