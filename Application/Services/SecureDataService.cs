using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using System.Threading.Tasks;

namespace home_pisos_vinilicos.Application.Services
{
    public interface ISecureDataService
    {
        Task<SecureDataDto> GetSecureDataAsync(string token);
    }

    public class SecureDataService : ISecureDataService
    {
        public async Task<SecureDataDto> GetSecureDataAsync(string token)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                var uid = decodedToken.Uid;
                // Lógica para obtener datos relacionados con el UID
                return new SecureDataDto { Message = "Secure data access granted", UserId = uid };
            }
            catch (FirebaseAuthException)
            {
                return null;
            }
        }
    }
}
