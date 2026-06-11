using Portofolio.DTOs.ExperienceDTOs;

namespace Portofolio.Services.ExperienceServices
{
    public interface IExperienceServices
    {

        Task<ResponseExperienceDTO> CreateExperienceAsync(CreateExperienceDTO createDto);

        Task<bool> DeleteExperienceAsync(int id);

        Task<List<ResponseExperienceDTO>> GetExperienceByProfileIdAsync(int id);

        Task<ResponseExperienceDTO?> UpdateExperienceAsync(int id, UpdateExperienceDTO updateDto);
    }
}
