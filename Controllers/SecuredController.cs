using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using System.Threading.Tasks;

namespace home_pisos_vinilicos.Application.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecuredController : ControllerBase
    {
        private readonly ISecureDataService _secureDataService;

        public SecuredController(ISecureDataService secureDataService)
        {
            _secureDataService = secureDataService;
        }

        [HttpGet("secure-data")]
        [Authorize]
        public async Task<IActionResult> GetSecureData()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var result = await _secureDataService.GetSecureDataAsync(token);

            if (result == null)
            {
                return Unauthorized("Invalid or expired token.");
            }

            return Ok(result);
        }
    }
}
