using Portofolio.DTOs.ImageDTOs;
using Portofolio.Models.ImageModel;

namespace Portofolio.Services.ImageServices
{
    public interface IImageServices
    {

    
      
        Task<List<ResponseImageDTO>> GetUserImagesOrderedByDateAsync(int profileId);
        Task<bool> DeleteProfileImageAsync(int id);

        Task<ImageData> UploadProfileImageAsync(UploadImgDTO dto);
    }
}
