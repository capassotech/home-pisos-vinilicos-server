using home_pisos_vinilicos.Application.Interfaces;

namespace home_pisos_vinilicos.Application.Services
{
    public class TokenService : ITokenService
    {
        private string _token;

        public string GetToken() => _token;
        public void SetToken(string token) => _token = token;
    }
}
