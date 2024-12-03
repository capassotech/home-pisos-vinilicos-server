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

        public override async Task<bool> Insert(Product newProduct, List<Stream> imageStreams)
        {
            try
            {
                var firebaseResult = await _firebaseClient
                    .Child("Product")
                    .PostAsync(newProduct);

                newProduct.IdProduct = firebaseResult.Key;

                if (imageStreams != null && imageStreams.Any())
                {
                    var imageUrls = new List<string>();
                    foreach (var imageStream in imageStreams)
                    {
                        string imageUrl = await UploadProductImageAsync(imageStream, newProduct.IdProduct);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            imageUrls.Add(imageUrl);
                        }
                        else
                        {
                            Console.WriteLine("No se pudo subir una de las imágenes.");
                        }
                    }

                    if (imageUrls.Any())
                    {
                        newProduct.ImageUrls = imageUrls;

                        await _firebaseClient
                            .Child("Product")
                            .Child(newProduct.IdProduct)
                            .PutAsync(newProduct);
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

        public async Task<List<string>> UploadProductImagesAsync(List<Stream> imageStreams, string idProduct)
        {
            var imageUrls = new List<string>();
            foreach (var imageStream in imageStreams)
            {
                string imageUrl = await UploadProductImageAsync(imageStream, idProduct);
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    imageUrls.Add(imageUrl);
                }
            }
            return imageUrls;
        }

        public override async Task<bool> Update(Product updateProduct, List<Stream> imageStreams)
        {
            try
            {
                if (imageStreams != null && imageStreams.Any())
                {
                    var newImageUrls = new List<string>();
                    foreach (var imageStream in imageStreams)
                    {
                        string newImageUrl = await UploadProductImageAsync(imageStream, updateProduct.IdProduct);
                        if (!string.IsNullOrEmpty(newImageUrl))
                        {
                            newImageUrls.Add(newImageUrl);
                        }
                        else
                        {
                            Console.WriteLine("No se pudo actualizar una de las imágenes.");
                        }
                    }

                    if (newImageUrls.Any())
                    {
                        updateProduct.ImageUrls = newImageUrls;
                    }
                    else
                    {
                        throw new Exception("No se pudo actualizar ninguna imagen.");
                    }
                }

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
                return await base.Delete(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateRangeAsync(IEnumerable<Product> products)
        {
            try
            {
                var tasks = products.Select(async product =>
                {
                    await _firebaseClient
                        .Child("Product")
                        .Child(product.IdProduct)
                        .PutAsync(product);
                });

                await Task.WhenAll(tasks);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar múltiples productos: {ex.Message}");
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

                // Intentar obtener la lista de URLs en lugar de un solo string
                var imageUrls = await _firebaseClient
                    .Child("Product")
                    .Child(id)
                    .Child("ImageUrl")
                    .OnceAsync<string>();

                // Si se obtuvieron URLs, mapearlas a una lista de strings
                if (imageUrls != null && imageUrls.Any())
                {
                    product.ImageUrls = imageUrls.Select(i => i.Object).ToList();
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
            var productsData = await _firebaseClient.Child("Product").OnceAsync<Product>();

            var products = new List<Product>();

            foreach (var p in productsData)
            {
                var product = p.Object;
                product.IdProduct = p.Key;
                products.Add(product);
            }

            return products;
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