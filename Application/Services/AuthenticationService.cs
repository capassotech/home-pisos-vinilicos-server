using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.Interfaces;
using System;
using System.Threading.Tasks;
using System.Net.Mail;

namespace home_pisos_vinilicos.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", new { Email = email, Password = password });

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                return "Login failed";
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
                UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
                return $"Successfully created new user: {userRecord.Uid}";
            }
            catch (FirebaseAuthException ex)
            {
                
                return $"Error creating new user: {ex.Message}";
            }
            catch (Exception ex)
            {
                return $"Unexpected error: {ex.Message}";
            }
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            // generar token de recuperación y enviarlo por correo
            var token = Guid.NewGuid().ToString(); // Token temporal de ejemplo

            // Ejemplo de envío de correo (SMTP)
            var mailMessage = new MailMessage("noreply@yourapp.com", email)
            {
                Subject = "Password Recovery",
                Body = $"Use this token to recover your password: {token}"
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
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}
