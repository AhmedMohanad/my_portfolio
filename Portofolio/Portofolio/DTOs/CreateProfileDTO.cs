using Portofolio.Models.UserModels;

namespace Portofolio.DTOs
{
    public class CreateProfileDTO
    {


        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
    }
}
