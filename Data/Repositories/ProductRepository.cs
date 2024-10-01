using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq.Expressions;
using System.Net.Http;
using System.Text;
using static AuthenticationService;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class ProductRepository : FirebaseRepository<Product>, IProductRepository
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly FirebaseStorage _firebaseStorage;

        public ProductRepository() : base()
        {
            _firebaseClient = new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/");

            _firebaseStorage = new FirebaseStorage("gs://home-pisos-vinilicos.appspot.com", new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(AuthenticationService.IdToken), // Usa la propiedad estática
                ThrowOnCancel = true
            });
        }

        public async Task<string> UploadProductImageAsync(Stream imageStream, string IdProduct)
        {
            try
            {
                var imagePath = $"products/{IdProduct}/{Guid.NewGuid()}.jpg"; // Ruta de la imagen
                await _firebaseStorage.Child(imagePath).PutAsync(imageStream); // Subir la imagen

                // Devuelve la URL de descarga
                return await _firebaseStorage.Child(imagePath).GetDownloadUrlAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                return string.Empty; // Devuelve cadena vacía en caso de error
            }
        }

        public override async Task<bool> Insert(Product newProduct, Stream? imageStream = null)
        {
            try
            {
                // Si se proporciona una imagen, sube la imagen a Firebase Storage
                if (imageStream != null)
                {
                    string imageUrl = await UploadProductImageAsync(imageStream, newProduct.IdProduct.ToString());
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        newProduct.Image = imageUrl; // Asigna la URL de la imagen al producto
                    }
                    else
                    {
                        throw new Exception("No se pudo subir la imagen."); // Lanza una excepción si falla la subida de imagen
                    }
                }

                // Inserta el producto en Firebase Realtime Database
                return await base.Insert(newProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar producto: {ex.Message}");
                return false;
            }
        }

        public override async Task<bool> Update(Product updateProduct, Stream? imageStream = null)
        {
            try
            {
                // Si se proporciona una nueva imagen, actualiza la imagen en Firebase Storage
                if (imageStream != null)
                {
                    string newImageUrl = await UploadProductImageAsync(imageStream, updateProduct.IdProduct.ToString());
                    if (!string.IsNullOrEmpty(newImageUrl))
                    {
                        updateProduct.Image = newImageUrl; // Actualiza la URL de la imagen
                    }
                    else
                    {
                        throw new Exception("No se pudo actualizar la imagen."); // Lanza una excepción si falla la subida de imagen
                    }
                }

                return await base.Update(updateProduct);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto: {ex.Message}");
                return false;
            }
        }


        public override async Task<bool> Delete(string id)
        {
            try
            {
                // Aquí podrías manejar la eliminación de la imagen en Firebase Storage si lo deseas
                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                return false;
            }
        }

        public async Task<Product?> GetByIdWithCategory(string id)
        {
            var product = await GetProductById(id);
            if (product == null)
            {
                return null;
            }

            var category = await GetCategoryById(product.IdCategory);
            if (category != null)
            {
                product.Category = category;
            }

            return product;
        }

        private async Task<Product?> GetProductById(string id)
        {
            return await _firebaseClient
                .Child("Product")
                .Child(id)
                .OnceSingleAsync<Product>();
        }

        private async Task<Category?> GetCategoryById(string? Idcategory)
        {
            if (string.IsNullOrEmpty(Idcategory))
            {
                return null;
            }

            return await _firebaseClient
                .Child("Category")
                .Child(Idcategory)
                .OnceSingleAsync<Category>();
        }

        public async Task<List<Product>> GetAllWithCategories()
        {
            try
            {
                var productList = await GetAllProducts();
                var categoryDictionary = await GetCategoryDictionary();

                AssignCategoriesToProducts(productList, categoryDictionary);

                return productList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Product>();
            }
        }

        private async Task<List<Product>> GetAllProducts()
        {
            var products = await _firebaseClient.Child("Product").OnceAsync<Product>();

            return products.Select(p =>
            {
                var product = p.Object;
                product.IdProduct = p.Key; // Asigna el ID del producto
                return product;
            }).ToList();
        }

        private async Task<Dictionary<string, Category>> GetCategoryDictionary()
        {
            var categories = await _firebaseClient.Child("Category").OnceAsync<Category>();
            return categories.ToDictionary(c => c.Key, c => c.Object);
        }

        private void AssignCategoriesToProducts(List<Product> products, Dictionary<string, Category> categoryDictionary)
        {
            foreach (var product in products)
            {
                if (!string.IsNullOrEmpty(product.IdCategory) && categoryDictionary.ContainsKey(product.IdCategory))
                {
                    product.Category = categoryDictionary[product.IdCategory]; // Asigna la categoría al producto
                }
            }
        }
    }
}
