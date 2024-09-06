using Firebase.Database;
using Firebase.Database.Query;
using home_pisos_vinilicos.Domain;
using Microsoft.AspNetCore.Mvc;

namespace home_pisos_vinilicos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirebaseTestController : ControllerBase
    {
        private readonly FirebaseClient _firebaseClient;

        public FirebaseTestController()
        {
            _firebaseClient = new FirebaseClient(
                "https://home-pisos-vinilicos-default-rtdb.firebaseio.com/"
            );
        }

        [HttpPost("add-product")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            try
            {
                var result = await _firebaseClient
                    .Child("products")
                    .PostAsync(product);

                return Ok(new { Id = result.Key });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al agregar producto: {ex.Message}");
            }
        }

        [HttpGet("get-product/{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            try
            {
                var product = await _firebaseClient
                    .Child("products")
                    .Child(id)
                    .OnceSingleAsync<Product>();

                if (product != null)
                {
                    return Ok(product);
                }
                else
                {
                    return NotFound("Producto no encontrado.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener producto: {ex.Message}");
            }
        }

        [HttpPut("update-product/{id}")]
        public async Task<IActionResult> UpdateProduct(string id, [FromBody] Product product)
        {
            try
            {
                await _firebaseClient
                    .Child("products")
                    .Child(id)
                    .PutAsync(product);

                return Ok("Producto actualizado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al actualizar producto: {ex.Message}");
            }
        }

        [HttpDelete("delete-product/{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                await _firebaseClient
                    .Child("products")
                    .Child(id)
                    .DeleteAsync();

                return Ok("Producto eliminado exitosamente.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al eliminar producto: {ex.Message}");
            }
        }
    }
}
