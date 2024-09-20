using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {


        private readonly SubCategoryService subCategoryService;


        public SubCategoryController(SubCategoryService subCategoryService)
        {
            this.subCategoryService = subCategoryService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<SubCategoryDto>>> GetSubCategorys()
        {
            var subCategorys = await subCategoryService.GetAllAsync();
            return Ok(subCategorys);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveSubCategory(SubCategoryDto subCategoryDto)
        {
            var result = await subCategoryService.SaveAsync(subCategoryDto);
            if (result)
            {
                return Ok("SubCategory guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el SubCategory.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<SubCategoryDto>> GetSubCategoryById(string id)
        {
            var subCategory = await subCategoryService.GetByIdAsync(id);
            if (subCategory == null)
            {
                return NotFound();
            }

            return Ok(subCategory);
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateSubCategoryById(string id, SubCategoryDto requestDto)
        {
            requestDto.IdSubCategory = id;
            Console.WriteLine(id);
            var result = await subCategoryService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("SubCategory actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el SubCategory.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteSubCategoryById(string id)
        {
            try
            {
                var result = await subCategoryService.DeleteAsync(id);
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

        [HttpGet("count/{categoryId}")]
        public async Task<IActionResult> GetSubCategoryCount(string categoryId)
        {
            var count = await subCategoryService.CountSubCategoriesByCategoryIdAsync(categoryId);
            return Ok(count);
        }
    }
}
