using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.Interfaces;
using System;
using System.Threading.Tasks;
using System.Net.Mail;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Models;

namespace home_pisos_vinilicos.Application.Services
{
    
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILoginRepository _loginRepository;  // Cambia a ILoginRepository
        private readonly HttpClient _httpClient;
        private readonly string firebaseApiKey = "AIzaSyDCjcyPOQ_29zyZGtxk13iJdbDsP1AG8bM";

        public AuthenticationService(ILoginRepository loginRepository, HttpClient httpClient)
        {
            _loginRepository = loginRepository;  
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            string firebaseAuthUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}";
            var payload = new
            {
                email = email.Trim().ToLower(), 
                password = password,
                returnSecureToken = true
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync(firebaseAuthUrl, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var usersFromDb = await _loginRepository.GetAll(u => u.Email == email.Trim().ToLower());

                    if (!usersFromDb.Any())
                    {
                        return "Usuario no encontrado en la base de datos.";
                    }

                    return responseContent; 
                }
                else
                {
                    return $"Login fallido: {responseContent}";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }



        public async Task<string> RegisterUserAsync(string email, string password)
        {
            var userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                Password = password,
            };

            try
            {
                // Crear un nuevo usuario en Firebase Authentication
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                // Crear una instancia de Login para almacenar en tu base de datos
                var newUser = new Login
                {
                    Email = email,
                    // Otros campos que quieras guardar
                };

                // Insertar el nuevo usuario en tu base de datos
                await _loginRepository.Insert(newUser);

                return $"Successfully created new user: {userRecord.Uid}";
            }
            catch (FirebaseAuthException ex)
            {
                // Manejar errores específicos de Firebase Auth
                return $"Error creating new user: {ex.Message}";
            }
            catch (Exception ex)
            {
                // Manejar errores generales
                return $"Unexpected error: {ex.Message}";
            }
        }

        
        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);
                return decodedToken != null;
            }
            catch (FirebaseAuthException ex)
            {
                Console.WriteLine($"Error validating token: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            try
            {
                var resetLink = await GeneratePasswordResetLinkAsync(email);
                if (resetLink != null)
                {
                    return await SendPasswordResetEmailAsync(email, resetLink);
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ForgotPasswordAsync: {ex.Message}");
                return false;
            }
        }

        private async Task<string?> GeneratePasswordResetLinkAsync(string email)
        {
            try
            {
                return "https://example.com/reset-password?token=some-token"; // Placeholder
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating password reset link: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> SendPasswordResetEmailAsync(string email, string resetLink)
        {
            // Ejemplo de envío de correo (SMTP)
            var mailMessage = new MailMessage("noreply@yourapp.com", email)
            {
                Subject = "Password Recovery",
                Body = $"Use this link to recover your password: {resetLink}"
            };

            using (var smtpClient = new SmtpClient("smtp.your-email-provider.com"))
            {
                smtpClient.Credentials = new System.Net.NetworkCredential("your-email@domain.com", "your-password");
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending password reset email: {ex.Message}");
                    return false;
                }
            }
        }
    
    }
    
}
