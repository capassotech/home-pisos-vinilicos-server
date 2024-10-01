using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;
using System.IO;

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
        public async Task<ActionResult> SaveProduct([FromForm] ProductDto productDto, [FromForm] IFormFile productImage)
        {
            Stream imageStream = null;

            // Verificar si se ha proporcionado una imagen
            if (productImage != null && productImage.Length > 0)
            {
                var memoryStream = new MemoryStream();
                await productImage.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // Reiniciar la posición del Stream
                imageStream = memoryStream;
            }

            var result = await productService.SaveAsync(productDto, imageStream);
            if (result)
            {
                return Ok("Producto guardado exitosamente.");
            }

            return BadRequest("No se pudo guardar el producto.");
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
        public async Task<ActionResult> UpdateProductById(string id, ProductDto requestDto, IFormFile productImage)
        {
            requestDto.IdProduct = id;

            // Manejar la carga de la imagen, si se proporciona
            Stream imageStream = null;
            if (productImage != null && productImage.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await productImage.CopyToAsync(memoryStream);
                    imageStream = memoryStream; // Asignar el stream a la variable
                }
            }

            var result = await productService.UpdateAsync(requestDto, imageStream);
            if (result)
            {
                return Ok("Producto actualizado exitosamente.");
            }
            return BadRequest("No se pudo actualizar el producto.");
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
