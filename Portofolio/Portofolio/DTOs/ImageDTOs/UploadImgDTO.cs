namespace Portofolio.DTOs.ImageDTOs
{
    public class UploadImgDTO
    {
        public int ProfileId { get; set; }
        public IFormFile? File { get; set; }
        public string? ImageUrl { get; set; }
    }
}
