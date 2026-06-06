using Portofolio.DTOs.ImageDTOs;

namespace Portofolio.Services.ImageServices
{
    public interface IImageServices
    {

        Task<ImageResponseDTO> UploadImageAsync(UploadImgDTO createDto);
        Task<ImageResponseDTO?> GetImageByIdAsync(int id);
        Task<List<ImageResponseDTO>> GetUserImagesOrderedByDateAsync(int profileId);
        Task<bool> DeleteImageAsync(int id);
    }
}
