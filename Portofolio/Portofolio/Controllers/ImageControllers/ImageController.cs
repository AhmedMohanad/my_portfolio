using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portofolio.DTOs.ImageDTOs;
using Portofolio.LoggingServices;
using Portofolio.Services.ImageServices;

namespace Portofolio.Controllers.ImageControllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : Controller
    {
        private readonly IImageServices _imageServices;
        private readonly ISimpleLogger _logger;

        public ImageController(IImageServices imageServices, ISimpleLogger logger)
        {
            _imageServices = imageServices;
            _logger = logger;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]  
        public async Task<IActionResult> UploadProfileImage([FromForm] UploadImgDTO dto)  //  FromForm 
        {
            _logger.LogInfo("Attempting to upload profile image");
            var result = await _imageServices.UploadProfileImageAsync(dto);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUserImagesOrderedByDate(int profileId)
        {
           
                _logger.LogInfo($"Getting images for profile ID: {profileId}");
                var images = await _imageServices.GetUserImagesOrderedByDateAsync(profileId);
                return Ok(images);
          

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileImage(int id)
        {
            
            
                _logger.LogInfo($"Attempting to delete image with ID: {id}");
                var result = await _imageServices.DeleteProfileImageAsync(id);
                if (!result)
                    return NotFound();

                return Ok();
            
            
        }

    }
}
