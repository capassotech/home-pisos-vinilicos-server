using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ContactController : ControllerBase
    {


        private readonly ContactService contactService;


        public ContactController(ContactService contactService)
        {
            this.contactService = contactService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<ContactDto>>> GetContacts()
        {
            var contacts = await contactService.GetAllAsync();
            return Ok(contacts);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveContact(ContactDto contactDto)
        {
            var result = await contactService.SaveAsync(contactDto);
            if (result)
            {
                return Ok("Product guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el Product.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ContactDto>> GetContactById(string id)
        {
            var contact = await contactService.GetByIdAsync(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateContactById(string id, ContactDto requestDto)
        {
            requestDto.IdContact = id;
            Console.WriteLine(id);
            var result = await contactService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("Product actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el Product.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteContactById(string id)
        {
            try
            {
                var result = await contactService.DeleteAsync(id);
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
