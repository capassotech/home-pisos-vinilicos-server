using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos.Pages;
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

            _firebaseStorage = new FirebaseStorage("home-pisos-vinilicos.appspot.com", new FirebaseStorageOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(AuthenticationService.IdToken),
                ThrowOnCancel = true
            });
        }

        public async Task<string> UploadProductImageAsync(Stream imageStream, string idProduct)
        {
            try
            {
                var imagePath = $"products/{idProduct}/{Guid.NewGuid()}.jpg";

                await _firebaseStorage.Child(imagePath).PutAsync(imageStream);

                var imageUrl = await _firebaseStorage.Child(imagePath).GetDownloadUrlAsync();

                if (string.IsNullOrEmpty(imageUrl))
                {
                    throw new Exception("No se pudo obtener la URL de la imagen.");
                }

                return imageUrl;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                return string.Empty;
            }
        }


        public override async Task<bool> Insert(Product newProduct, Stream? imageStream = null)
        {
            try
            {
                // Primero, insertamos el producto para obtener su ID
                var firebaseResult = await _firebaseClient
                    .Child("Product")
                    .PostAsync(newProduct);

                // Asignamos el ID generado a la propiedad IdProduct
                newProduct.IdProduct = firebaseResult.Key;

                // Si hay un Stream de imagen, subimos la imagen y actualizamos el producto
                if (imageStream != null)
                {
                    string imageUrl = await UploadProductImageAsync(imageStream, newProduct.IdProduct);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        newProduct.ImageUrl = imageUrl;

                        // Actualizamos el producto completo, incluyendo la URL de la imagen
                        await _firebaseClient
                            .Child("Product")
                            .Child(newProduct.IdProduct)
                            .PutAsync(newProduct);
                    }
                    else
                    {
                        Console.WriteLine("No se pudo subir la imagen.");
                    }
                }

                return true;
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
                    string newImageUrl = await UploadProductImageAsync(imageStream, updateProduct.IdProduct);
                    if (!string.IsNullOrEmpty(newImageUrl))
                    {
                        updateProduct.ImageUrl = newImageUrl; // Actualiza la URL de la imagen
                    }
                    else
                    {
                        throw new Exception("No se pudo actualizar la imagen.");
                    }
                }

                // Actualiza el producto completo en Firebase
                await _firebaseClient
                    .Child("Product")
                    .Child(updateProduct.IdProduct)
                    .PutAsync(updateProduct);

                return true;
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
            var product = await _firebaseClient
                .Child("Product")
                .Child(id)
                .OnceSingleAsync<Product>();

            if (product != null)
            {
                product.IdProduct = id;
                // Asegúrate de que ImageUrl se incluya aquí si no está ya
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
                    // Intenta obtener ImageUrl específicamente si no está incluido
                    var imageUrl = await _firebaseClient
                        .Child("Product")
                        .Child(id)
                        .Child("ImageUrl")
                        .OnceSingleAsync<string>();
                    product.ImageUrl = imageUrl;
                }
            }

            return product;
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
                                           // Asegúrate de que ImageUrl se incluya aquí si no está ya
                if (p.Object.ImageUrl != null)
                {
                    product.ImageUrl = p.Object.ImageUrl;
                }
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
