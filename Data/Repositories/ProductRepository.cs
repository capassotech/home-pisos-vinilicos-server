using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;

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
                var firebaseResult = await _firebaseClient
                    .Child("Product")
                    .PostAsync(newProduct);

                newProduct.IdProduct = firebaseResult.Key;

                if (imageStream != null)
                {
                    string imageUrl = await UploadProductImageAsync(imageStream, newProduct.IdProduct);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        newProduct.ImageUrl = imageUrl;

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
                if (newProduct.Colors != null && newProduct.Colors.Any())
                {
                    foreach (var color in newProduct.Colors)
                    {
                        // Inserta cada color asociado al producto en Firebase
                        await _firebaseClient
                            .Child("Product")
                            .Child(newProduct.IdProduct)
                            .Child("Colors")
                            .PostAsync(color);
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
                if (imageStream != null)
                {
                    string newImageUrl = await UploadProductImageAsync(imageStream, updateProduct.IdProduct);
                    if (!string.IsNullOrEmpty(newImageUrl))
                    {
                        updateProduct.ImageUrl = newImageUrl;
                    }
                    else
                    {
                        throw new Exception("No se pudo actualizar la imagen.");
                    }
                }

                await _firebaseClient
                    .Child("Product")
                    .Child(updateProduct.IdProduct)
                    .PutAsync(updateProduct);
                if (updateProduct.Colors != null && updateProduct.Colors.Any())
                {
                    // Borra la lista actual de colores en Firebase para evitar duplicados o inconsistencias
                    await _firebaseClient
                        .Child("Product")
                        .Child(updateProduct.IdProduct)
                        .Child("Colors")
                        .DeleteAsync();

                    // Inserta los nuevos colores en Firebase
                    foreach (var color in updateProduct.Colors)
                    {
                        await _firebaseClient
                            .Child("Product")
                            .Child(updateProduct.IdProduct)
                            .Child("Colors")
                            .PostAsync(color);
                    }
                }
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
                if (string.IsNullOrEmpty(product.ImageUrl))
                {
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
                product.IdProduct = p.Key;

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
                    product.Category = categoryDictionary[product.IdCategory]; // Asigna la categor�a al producto
                }
            }
        }
    }
}