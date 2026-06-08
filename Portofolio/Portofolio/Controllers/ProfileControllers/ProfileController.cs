using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portofolio.Data;
using Portofolio.DTOs.ProfileDTOs;
using Portofolio.Services.ProfileServices;

namespace Portofolio.Controllers.ProfileControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProfileController> _logger;
        private readonly IProfileServices _profileServices;
        public ProfileController(ApplicationDbContext context, ILogger<ProfileController> logger, IProfileServices profileServices)
        {
            _context = context;
            _logger = logger;
            _profileServices = profileServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProfile(CreateProfileDTO dto)
        {
            try
            {
                _logger.LogInformation("Creating profile request for {Email}", dto.Email);
                var result = await _profileServices.CreateProfileAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating profile for {Email}", dto.Email);
                return BadRequest(ex.Message);
            }


        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProfileById(int id)
        {
            try
            {
                _logger.LogInformation("Getting profile by ID: {ProfileId}", id);
                var result = await _profileServices.GetProfileByIdAsync(id);
                if (result == null)
                {
                    _logger.LogWarning("Profile not found with ID: {ProfileId}", id);
                    return NotFound("Profile not found");
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting profile with ID: {ProfileId}", id);
                return BadRequest(ex.Message);
            }

        }

    }
}
