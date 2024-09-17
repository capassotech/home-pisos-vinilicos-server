

using home_pisos_vinilicos.Application.DTOs;

namespace home_pisos_vinilicos.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> RegisterAsync(string email, string password);
        Task<AuthResult> LoginAsync(string email, string password);
        Task<string> ForgotPasswordAsync(string email);
        Task<bool> LogoutAsync(string idToken);
        Task<bool> IsUserAuthenticated(string idToken);
        //Task<bool> ValidateTokenAsync(string token);
    }
}
