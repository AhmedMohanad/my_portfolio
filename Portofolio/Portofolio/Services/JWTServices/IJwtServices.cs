using Portofolio.Models.UserModels;

namespace Portofolio.Services.JWTServices
{
    public interface IJwtServices
    {
        string GenerateJwtToken(Profile user);
    }
}
