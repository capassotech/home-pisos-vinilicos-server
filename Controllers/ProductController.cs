using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Shared.DTOs;

namespace home_pisos_vinilicos.Controllers
{
    [ApiController]
    [Route("api/producto")]
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
    }
}
