using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {


        private readonly CategoryService categoryService;


        public CategoryController(CategoryService categoryService)
        {
            this.categoryService = categoryService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategorys()
        {
            var categorys = await categoryService.GetAllAsync();
            return Ok(categorys);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveCategory(CategoryDto categoryDto)
        {
            var result = await categoryService.SaveAsync(categoryDto);
            if (result)
            {
                return Ok("Category guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el Category.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategoryById(string id)
        {
            var category = await categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateCategoryById(string id, [FromBody] CategoryDto requestDto)
        {
            requestDto.IdCategory = id;  // Asegura que el ID esté asignado
            var result = await categoryService.UpdateAsync(requestDto);

            if (result)
            {
                return Ok("Categoría actualizada exitosamente.");
            }

            return BadRequest("No se pudo actualizar la categoría.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteCategoryById(string id)
        {
            try
            {
                var result = await categoryService.DeleteAsync(id);
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

        [HttpPut("{id}/feature")]
        public async Task<ActionResult> SetFeaturedCategory(string id, [FromBody] bool isFeatured)
        {
            try
            {
                var result = await categoryService.SetFeaturedCategoryAsync(id, isFeatured);
                if (result)
                {
                    return Ok("Categoría actualizada como destacada.");
                }
                return BadRequest("No se pudo actualizar la categoría.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message); // Mensaje de error si ya hay 2 categorías destacadas
            }
        }


    }
}
