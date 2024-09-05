using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace home_pisos_vinilicos.ServicesFront
{
    public class ProductServiceFront
    {
        private readonly HttpClient httpClient;

        public ProductServiceFront(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<List<Product>> GetProductsAsync()
        {
            var response = await httpClient.GetFromJsonAsync<List<Product>>("api/producto");
            return response;
        }

        public async Task<Product> GetProductByIdAsync(string id)
        {
            var response = await httpClient.GetFromJsonAsync<Product>($"api/producto/{id}");
            return response;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            var response = await httpClient.PostAsJsonAsync("api/producto/new", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UpdateProductAsync(string id, Product product)
        {
            var response = await httpClient.PutAsJsonAsync($"api/producto/update/{id}", product);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteProductAsync(string id)
        {
            var response = await httpClient.DeleteAsync($"api/producto/delete/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
