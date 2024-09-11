

namespace home_pisos_vinilicos.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> LoginAsync(string email, string password);
        Task<string> RegisterUserAsync(string email, string password);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ValidateTokenAsync(string token);
    }
}
