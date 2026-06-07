using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portofolio.DTOs.AuthDTOs;
using Portofolio.Services.AuthServices;

namespace Portofolio.Controllers.AuthControllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase 
    {
        private readonly IAuthServices _authServices;

        public AuthController(IAuthServices auth) 
        {
            _authServices = auth;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authServices.LoginAsync(dto);
            return Ok(new { message = "Logged in successfully.", token });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var token = Request.Headers["Authorization"]
                .ToString().Replace("Bearer ", "");

            await _authServices.LogoutAsync(token);
            return Ok(new { message = "Logged out successfully." });
        }
    }
}