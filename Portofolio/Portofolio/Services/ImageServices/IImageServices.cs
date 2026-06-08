using Portofolio.DTOs.ImageDTOs;
using Portofolio.Models.ImageModel;

namespace Portofolio.Services.ImageServices
{
    public interface IImageServices
    {

        Task<ResponseImageDTO> UploadImageAsync(UploadImgDTO createDto);
        Task<ResponseImageDTO?> GetImageByIdAsync(int id);
        Task<List<ResponseImageDTO>> GetUserImagesOrderedByDateAsync(int profileId);
        Task<bool> DeleteImageAsync(int id);

        Task<ImageData> UploadProfileImageAsync(UploadImgDTO dto);
    }
}
