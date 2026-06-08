using Portofolio.Data;
using Portofolio.DTOs.ExperienceDTOs;
using Portofolio.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace Portofolio.Services.ExperienceServices
{
    public class ExperienceServices : IExperienceServices
    {
        private readonly ApplicationDbContext _context;

        public ExperienceServices(ApplicationDbContext context) { _context = context; }

        public async Task<ResponseExperienceDTO> CreateExperienceAsync(CreateExperienceDTO createDto)
        {
            var experience = new Experience
            {
                Company = createDto.Company,
                Role = createDto.Role,
                Description = createDto.Description,
                StartDate = createDto.StartDate,
                EndDate = createDto.EndDate,
                ProfileId = createDto.ProfileId
            };
            _context.Experiences.Add(experience);
            await _context.SaveChangesAsync();
            return new ResponseExperienceDTO
            {
                Id = experience.Id,
                Company = experience.Company,
                Role = experience.Role,
                Description = experience.Description,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate
            };
        }

        public async Task<bool> DeleteExperienceAsync(int id)
        {
            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null) return false;
            _context.Experiences.Remove(experience);
            await _context.SaveChangesAsync();
            return true;


        }

        public async Task<List<ResponseExperienceDTO>> GetExperienceByProfileIdAsync(int id)
        {
            var experiences = await _context.Experiences
                .Where(e => e.ProfileId == id)
                .Select(e => new ResponseExperienceDTO
                {
                    Id = e.Id,
                    Company = e.Company,
                    Role = e.Role,
                    Description = e.Description,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate
                })
                .ToListAsync();

            return experiences;
        }

        public async Task<ResponseExperienceDTO?> UpdateExperienceAsync(int id, UpdateExperienceDTO updateDto)
        {
            var experience = await _context.Experiences.FindAsync(id);
            if (experience == null) return null;

            if (updateDto.Company != null)
                experience.Company = updateDto.Company;

            if (updateDto.Role != null)
                experience.Role = updateDto.Role;

            if (updateDto.Description != null)
                experience.Description = updateDto.Description;

            if (updateDto.StartDate.HasValue)
                experience.StartDate = updateDto.StartDate.Value;

            if (updateDto.EndDate.HasValue)
                experience.EndDate = updateDto.EndDate.Value;

            if (updateDto.ProfileId != 0)
                experience.ProfileId = updateDto.ProfileId;

            await _context.SaveChangesAsync();

            return new ResponseExperienceDTO
            {
                Id = experience.Id,
                Company = experience.Company,
                Role = experience.Role,
                Description = experience.Description,
                StartDate = experience.StartDate,
                EndDate = experience.EndDate
            };
        }
    }
}
