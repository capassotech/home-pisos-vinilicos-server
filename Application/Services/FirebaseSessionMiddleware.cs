using FirebaseAdmin.Auth;

namespace home_pisos_vinilicos.Application.Services
{
    public class FirebaseSessionMiddleware
    {
        private readonly RequestDelegate _next;

        public FirebaseSessionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("No token provided.");
                return;
            }

            try
            {
                // Verificar el token JWT con Firebase
                FirebaseToken decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(token);

                // Si es válido, agregar el token decodificado al contexto
                context.Items["User"] = decodedToken;
                await _next(context);  // Continuar con la solicitud
            }
            catch (FirebaseAuthException ex)
            {
                // Token inválido o expirado
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync($"Invalid token: {ex.Message}");
            }
        }
    }
}
