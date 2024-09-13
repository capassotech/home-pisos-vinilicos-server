using FirebaseAdmin.Auth;
using home_pisos_vinilicos.Application.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace home_pisos_vinilicos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Token no proporcionado.");
            }

            var idToken = authHeader.Substring("Bearer ".Length);

            try
            {
                // Verifica el token de Firebase para validar la autenticación
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;

                var user = await FirebaseAuth.DefaultInstance.GetUserAsync(uid);
                SesionDto sesionDTO = new SesionDto
                {
                    Correo = user.Email // Validamos el token y obtenemos el correo
                };

                return StatusCode(StatusCodes.Status200OK, sesionDTO);
            }
            catch (FirebaseAuthException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Token inválido.");
            }
        }


        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Token no proporcionado.");
            }

            var idToken = authHeader.Substring("Bearer ".Length);

            try
            {
                // Verifica el token de Firebase
                var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
                string uid = decodedToken.Uid;

                // Revoca todos los tokens activos emitidos para este usuario
                await FirebaseAuth.DefaultInstance.RevokeRefreshTokensAsync(uid);

                return StatusCode(StatusCodes.Status200OK, "Sesión cerrada correctamente.");
            }
            catch (FirebaseAuthException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized, "Error al cerrar la sesión.");
            }
        }


    }
}
