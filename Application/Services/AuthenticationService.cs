using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos.Domain.Models;
using System.Net.Mail;

public class AuthenticationService : IAuthenticationService
{
    private readonly ILoginRepository _loginRepository; 
    private readonly HttpClient _httpClient;

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
            var user = await FirebaseAuth.DefaultInstance.GetUserByEmailAsync(email);

            if (user == null)
            {
                return new AuthResult
                {
                    IsSuccess = false,
                    Message = "El usuario no fue encontrado."
                };
            }
            var token = await FirebaseAuth.DefaultInstance.CreateCustomTokenAsync(user.Uid);

            return new AuthResult
            {
                IsSuccess = true,
                Message = "Inicio de sesión exitoso.",
                Token = token
            };
        }
        catch (FirebaseAuthException ex)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = $"Error de autenticación: {ex.Message}"
            };
        }
        catch (Exception ex)
        {
            return new AuthResult
            {
                IsSuccess = false,
                Message = $"Error inesperado: {ex.Message}"
            };
        }
    }

    public async Task<string> ForgotPasswordAsync(string email)
    {
        try
        {
            var actionCodeSettings = new ActionCodeSettings()
            {
                // URL a la que se redirigirá después de que el usuario restablezca su contraseña
                Url = "https://tudominio.com/reset-password",
            };

            var resetPasswordLink = await FirebaseAuth.DefaultInstance.GeneratePasswordResetLinkAsync(email, actionCodeSettings);
            return resetPasswordLink;
        }
        catch (Exception ex)
        {
            return null;
        }
    }
    public async Task<bool> LogoutAsync(string idToken)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
            string uid = decodedToken.Uid;

            await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(uid);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }




}



public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public string Token { get; set; }
}










