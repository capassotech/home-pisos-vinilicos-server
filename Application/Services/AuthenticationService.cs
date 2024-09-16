using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos.Domain.Models;
using System.Net.Mail;

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






    public async Task<string> RegisterAsync(string email, string password)
    {
        try
        {
            var userRecordArgs = new UserRecordArgs()
            {
                Email = email,
                Password = password,
                EmailVerified = false,
                Disabled = false,
            };

            var userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);
            return $"User created successfully: {userRecord.Uid}";
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }


    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            // Obtenemos el usuario por correo electrónico
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            if (user != null)
            {
                var token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

                return new AuthResult
                {
                    IsSuccess = true,
                    Message = "User logged in successfully.",
                    Token = token
                };
            }

            return new AuthResult
            {
                IsSuccess = false,
                Message = "User not found."
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = $"Error: {ex.Message}"
            };
        }
    }
}

public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
}











/*




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
            // Lógica para generar el enlace de restablecimiento de contraseña
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
*/