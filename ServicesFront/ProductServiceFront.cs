using home_pisos_vinilicos.Domain; // Asegúrate de que este espacio de nombres es correcto
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace home_pisos_vinilicos.ServicesFront
{
    public class ProductServiceFront
    {
        private readonly HttpClient httpClient;

        public ProductServiceFront(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://localhost:7223/");
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>("api/product/getAll");
            return response;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"api/product/{id}");
            return response;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            var response = await httpClient.PostAsJsonAsync("api/product/new", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(string id, Product product)
        {
            var response = await httpClient.PutAsJsonAsync($"api/product/update/{id}", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var response = await httpClient.DeleteAsync($"api/product/delete/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
