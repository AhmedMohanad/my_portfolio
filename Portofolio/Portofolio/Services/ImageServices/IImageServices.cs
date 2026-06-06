using Portofolio.DTOs.ImageDTOs;

namespace Portofolio.Services.ImageServices
{
    public interface IImageServices
    {

        Task<ResponseImageDTO> UploadImageAsync(UploadImgDTO createDto);
        Task<ResponseImageDTO?> GetImageByIdAsync(int id);
        Task<List<ResponseImageDTO>> GetUserImagesOrderedByDateAsync(int profileId);
        Task<bool> DeleteImageAsync(int id);
    }
}
