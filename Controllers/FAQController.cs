using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class FAQController : ControllerBase
    {


        private readonly FAQService fAQService;


        public FAQController(FAQService fAQService)
        {
            this.fAQService = fAQService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<FAQDto>>> GetFAQs()
        {
            var fAQs = await fAQService.GetAllAsync();
            return Ok(fAQs);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveFAQ(FAQDto fAQDto)
        {
            var result = await fAQService.SaveAsync(fAQDto);
            if (result)
            {
                return Ok("SubCategory guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el SubCategory.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<FAQDto>> GetFAQById(string id)
        {
            var subCategory = await fAQService.GetByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return Ok(subCategory);
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateFAQById(string id, FAQDto requestDto)
        {
            requestDto.IdFAQ = id;
            Console.WriteLine(id);
            var result = await fAQService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("SubCategory actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el SubCategory.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteFAQById(string id)
        {
            try
            {
                var result = await fAQService.DeleteAsync(id);
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
