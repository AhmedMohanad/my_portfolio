using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portofolio.DTOs.AuthDTOs;
using Portofolio.LoggingServices;
using Portofolio.Services.AuthServices;

namespace Portofolio.Controllers.AuthControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly IAuthServices _authServices;
        private readonly ISimpleLogger _logger;

        public AuthController(IAuthServices auth, ISimpleLogger logger) 
        {
            _authServices = auth;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            _logger.LogInfo($"Register request for: {dto.Email}");
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var token = await _authServices.RegisterAsync(dto);
                return Ok(new { message = "Registered successfully.", token });

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });

            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            _logger.LogInfo($"Login request for: {dto.Email}");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authServices.LoginAsync(dto);
            return Ok(new { message = "Logged in successfully.", token });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            _logger.LogInfo($"Logout request received.");
            var token = Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            await _authServices.LogoutAsync(token);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}