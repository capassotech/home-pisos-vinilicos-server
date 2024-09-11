using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;

namespace home_pisos_vinilicos.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {


        private readonly ProductService productService;


        public ProductController(ProductService productService)
        {
            this.productService = productService;

        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            var products = await productService.GetAllAsync();
            return Ok(products);
        }


        [HttpPost("new")]
        public async Task<ActionResult> SaveProduct(ProductDto productDto)
        {
            var result = await productService.SaveAsync(productDto);
            if (result)
            {
                return Ok("Product guardado exitosamente.");
            }
            return BadRequest("No se pudo guardar el Product.");
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            var product = await productService.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }


        [HttpPut("update/{id}")]
        public async Task<ActionResult> UpdateProductById(string id, ProductDto requestDto)
        {
            requestDto.IdProduct = id;
            Console.WriteLine(id);
            var result = await productService.UpdateAsync(requestDto);
            if (result)
            {
                return Ok("Product actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el Product.");
        }


        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            try
            {
                var result = await productService.DeleteAsync(id);
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


        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDto>>> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("El término de búsqueda no puede estar vacío.");
            }

            try
            {
                var products = await productService.SearchAsync(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

    }
}
