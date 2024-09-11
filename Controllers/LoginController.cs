using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace home_pisos_vinilicos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly LoginService _loginService;

        public LoginController(LoginService loginService)
        {
            _loginService = loginService;
        }

        // Obtener todos los logins
        [HttpGet("getAll")]
        public async Task<ActionResult<List<LoginDto>>> GetLogins()
        {
            var logins = await _loginService.GetAllAsync();
            return Ok(logins);
        }

        // Obtener un usuario por correo electrónico
        [HttpGet("getLoginByEmail")]
        public async Task<ActionResult<LoginDto>> GetLoginByEmail([FromQuery] string email)
        {
            try
            {
                var userRecord = await _loginService.GetUserByEmailAsync(email);
                var userRecordDto = new LoginDto
                {
                    Email = userRecord.Email
               
                };
                return Ok(userRecordDto);
            }
            catch (Exception ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
