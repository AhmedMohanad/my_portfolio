using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portofolio.DTOs.ExperienceDTOs;
using Portofolio.LoggingServices;
using Portofolio.Services.ExperienceServices;

namespace Portofolio.Controllers.ExperienceControllers
{

    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExperienceController : Controller
    {

        private readonly IExperienceServices _ExperienceService;
        private readonly ISimpleLogger _logger;

        public ExperienceController(IExperienceServices experienceService, ISimpleLogger logger)
        {
            _ExperienceService = experienceService;
            _logger = logger;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExperience(int id)
        {
            var experience = await _ExperienceService.GetExperienceByProfileIdAsync(id);
            if (experience == null)
            {
                _logger.LogInfo($"Experience with id {id} not found.");
                return NotFound(new { message = "No experience found" });
            }
            _logger.LogInfo($"Experience with id {id} retrieved successfully.");
            return Ok(experience);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExperience([FromBody] CreateExperienceDTO createDto)
        {
            var createdExperience = await _ExperienceService.CreateExperienceAsync(createDto);
            _logger.LogInfo($"Experience for profile id {createDto.ProfileId} created successfully.");
            return CreatedAtAction(nameof(GetExperience), new { id = createdExperience.Id }, createdExperience);
        }

        [HttpPut("id")]
        public async Task<IActionResult> UpdateExperience(int id, [FromBody] UpdateExperienceDTO updateDto)
        {
            var experience = await _ExperienceService.GetExperienceByProfileIdAsync(id);
            if (experience == null)
            {
                _logger.LogInfo($"Experience with id {id} not found.");
                return NotFound(new { message = "Experience not found" });
            }

            await _ExperienceService.UpdateExperienceAsync(id, updateDto);
            _logger.LogInfo($"Experience with id {id} updated successfully.");
            return NoContent();
        }

        [HttpDelete("id")]
        public async Task<IActionResult> DeleteExperience(int id)
        {
            var experience = await _ExperienceService.GetExperienceByProfileIdAsync(id);
            if (experience == null)
            {
                _logger.LogInfo($"Experience with id {id} not found.");
                return NotFound(new { message = "Experience not found" });
            }

            await _ExperienceService.DeleteExperienceAsync(id);
            _logger.LogInfo($"Experience with id {id} deleted successfully.");
            return NoContent();
        } 

    }
}
