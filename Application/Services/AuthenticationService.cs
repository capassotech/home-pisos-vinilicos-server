using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos.Domain.Models;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Text;

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
        var apiKey = "AIzaSyDCjcyPOQ_29zyZGtxk13iJdbDsP1AG8bM"; 
        var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

        var requestBody = new
        {
            email = email,
            password = password,
            returnSecureToken = true
        };

        var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseString);
            return new AuthResult
            {
                IsSuccess = true,
                Message = "Inicio de sesión exitoso.",
                Token = authResponse.IdToken
            };
        }
        else
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseString);
            return new AuthResult
            {
                IsSuccess = false,
                Message = $"Error de autenticación: {errorResponse}"
            };
        }
    }

    public class AuthResponse
    {
        [JsonProperty("idToken")]
        public string IdToken { get; set; }
    }

    public class ErrorResponse
    {
        public class Error
        {
            public string Message { get; set; }
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










