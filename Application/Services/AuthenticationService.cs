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
