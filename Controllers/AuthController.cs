using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application.Interfaces;
using home_pisos_vinilicos.Domain.Models;
using home_pisos_vinilicos.Application.DTOs;
using FirebaseAdmin.Auth;

namespace home_pisos_vinilicos.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Server is up and running!");
        }

        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello from the TestController!");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authenticationService.LoginAsync(request.Email, request.Password);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return Unauthorized(result.Message);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var result = await _authenticationService.RegisterAsync(request.Email, request.Password);
            return Ok(result);
        }

        public class RegisterRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        public class LoginRequest
        {
            public string Email { get; set; }
            public string Password { get; set; }
        }

        /*

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
        {
            var result = await _authenticationService.ForgotPasswordAsync(request.Email);

            if (result)
            {
                return Ok("Password recovery email sent successfully.");
            }
            else
            {
                return BadRequest("Failed to send recovery email.");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var idToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (idToken != null)
            {
                try
                {
                    var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                    await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(decodedToken.Uid);
                    return Ok("Logout successful.");
                }
                catch
                {
                    return Unauthorized("Invalid token");
                }
            }

            return Unauthorized("No token provided");
        }
        */

    }

    
}
