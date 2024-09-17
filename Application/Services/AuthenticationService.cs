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










