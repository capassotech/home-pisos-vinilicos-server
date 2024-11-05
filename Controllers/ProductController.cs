using Microsoft.AspNetCore.Mvc;
using home_pisos_vinilicos.Application.Services;
using home_pisos_vinilicos.Application.DTOs;

namespace home_pisos_vinilicos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("getAll")]
        public async Task<ActionResult<List<ProductDto>>> GetProducts()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPost("new")]
        public async Task<ActionResult<ProductDto>> SaveProduct([FromForm] ProductDto productDto, List<IFormFile> productImages)
        {
            try
            {
                var imageStreams = new List<Stream>();
                foreach (var image in productImages)
                {
                    imageStreams.Add(image.OpenReadStream());
                }
                var savedProduct = await _productService.SaveAsync(productDto, imageStreams);
                return CreatedAtAction(nameof(GetProductById), new { id = savedProduct.IdProduct }, savedProduct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(string id)
        {
            try
            {
                var product = await _productService.GetByIdAsync(id);
                if (product == null)
                {
                    return NotFound($"No se encontró el producto con ID {id}");
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpPut("update/{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProductById(string id, [FromForm] ProductDto productDto, List<IFormFile> productImages)
        {
            if (id != productDto.IdProduct)
            {
                return BadRequest("El ID en la URL no coincide con el ID del producto");
            }

            try
            {
                var imageStreams = new List<Stream>();
                foreach (var image in productImages)
                {
                    imageStreams.Add(image.OpenReadStream());
                }
                var updatedProduct = await _productService.UpdateAsync(productDto, imageStreams);
                return Ok(updatedProduct);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public async Task<ActionResult> DeleteProductById(string id)
        {
            try
            {
                var result = await _productService.DeleteAsync(id);
                if (result)
                {
                    return Ok("Producto eliminado exitosamente");
                }
                return NotFound($"No se encontró el producto con ID {id}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ProductDto>>> SearchProducts([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("El término de búsqueda no puede estar vacío.");
            }

            try
            {
                var products = await _productService.SearchAsync(query);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }
    }
}