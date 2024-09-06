

namespace home_pisos_vinilicos.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<string> LoginAsync(string email, string password);
    }
}
