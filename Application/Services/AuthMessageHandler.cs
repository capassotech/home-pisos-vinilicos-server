using home_pisos_vinilicos.Application.Interfaces;
using Microsoft.JSInterop;

namespace home_pisos_vinilicos.Application.Services
{
    public class AuthMessageHandler : DelegatingHandler
    {
        private readonly ITokenService _tokenService;

        public AuthMessageHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = _tokenService.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
