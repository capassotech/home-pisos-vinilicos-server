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
        public async Task<ActionResult> UpdateCategoryById(string id, CategoryDto requestDto)
        {
            requestDto.IdCategory = id;
            Console.WriteLine(id);
            var result = await categoryService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("Category actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el Category.");
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



    }
}
