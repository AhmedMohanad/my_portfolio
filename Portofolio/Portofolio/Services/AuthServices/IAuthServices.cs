using Portofolio.DTOs.AuthDTOs;

public interface IAuthServices
{
    Task<string?> RegisterAsync(RegisterDTO dto);
    Task<string?> LoginAsync(LoginDTO dto);
    Task LogoutAsync(string token);
}