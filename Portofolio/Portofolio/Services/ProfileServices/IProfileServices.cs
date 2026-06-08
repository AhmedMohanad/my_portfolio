using Portofolio.DTOs.ProfileDTOs;

namespace Portofolio.Services.ProfileServices
{
    public interface IProfileServices
    {
        Task<ResponseProfileDTO> CreateProfileAsync(CreateProfileDTO createDto);
        Task<ResponseProfileDTO?> GetProfileByIdAsync(int id);
    }
}
