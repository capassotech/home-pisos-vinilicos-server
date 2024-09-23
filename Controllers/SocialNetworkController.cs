using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SocialNetworkController : ControllerBase
    {


        private readonly SocialNetworkService socialNetworkService;


        public SocialNetworkController(SocialNetworkService socialNetworkService)
        {
            this.socialNetworkService = socialNetworkService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<SocialNetworkDto>>> GetSocialNetworks()
        {
            var contacts = await socialNetworkService.GetAllAsync();
            return Ok(contacts);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveSocialNetwork(SocialNetworkDto socialNetworkDto)
        {
            var result = await socialNetworkService.SaveAsync(socialNetworkDto);
            if (result)
            {
                return Ok("SocialNetwork guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el SocialNetwork.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SocialNetworkDto>> GetSocialNetworkById(string id)
        {
            var contact = await socialNetworkService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateSocialNetworkById(string id, SocialNetworkDto requestDto)
        {
            requestDto.IdSocialNetwork = id;
            Console.WriteLine(id);
            var result = await socialNetworkService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("SocialNetwork actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el SocialNetwork.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteSocialNetworkById(string id)
        {
            try
            {
                var result = await socialNetworkService.DeleteAsync(id);
                if (result)
                {
                    return Ok("Registro eliminado :)");
                }
                return BadRequest("No se pudo eliminar el registro.");
            }
            catch (Exception ex)
            {
                return BadRequest($"No se pudo eliminar el registro. El error es: {ex.Message}");
            }
        }

    }
}
