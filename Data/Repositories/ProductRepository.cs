using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;

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

        public async Task<(bool Success, string Url)> UploadProductImageAsync(Stream imageStream, string idProduct, CancellationToken cancellationToken = default)
        {
            try
            {
                var imagePath = $"Product/{idProduct}/{Guid.NewGuid()}.jpg";

                await RetryHelper.ExecuteWithRetry(async () =>
                {
                    await _firebaseStorage.Child(imagePath).PutAsync(imageStream, cancellationToken);
                }, maxRetries: 3);

                var imageUrl = await _firebaseStorage.Child(imagePath).GetDownloadUrlAsync();

                if (string.IsNullOrEmpty(imageUrl))
                {
                    throw new Exception("No se pudo obtener la URL de la imagen.");
                }

                return (true, imageUrl);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("La carga de la imagen se canceló debido a que excedió el tiempo límite.");
                return (false, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                return (false, string.Empty);
            }
        }

        public async Task<List<string>> UploadProductImagesAsync(List<Stream> imageStreams, string idProduct, CancellationToken cancellationToken = default)
        {
            var imageUrls = new List<string>();

            var tasks = imageStreams.Select(stream => UploadProductImageAsync(stream, idProduct, cancellationToken));
            var results = await Task.WhenAll(tasks);

            foreach (var result in results)
            {
                if (result.Success)
                {
                    imageUrls.Add(result.Url);
                }
            }

            return imageUrls;
        }

        public override async Task<bool> Insert(Product newProduct, List<Stream> imageStreams)
        {
            try
            {
                // Insertar producto sin imágenes primero
                var firebaseResult = await _firebaseClient
                    .Child("Product")
                    .PostAsync(newProduct);

                newProduct.IdProduct = firebaseResult.Key;

                // Subir las imágenes solo si se pasaron
                if (imageStreams != null && imageStreams.Any())
                {
                    // Subir imágenes
                    var imageUrls = await UploadProductImagesAsync(imageStreams, newProduct.IdProduct);

                    // Asignar las URLs de las imágenes al producto
                    newProduct.ImageUrls = imageUrls;

                    // Actualizar producto con las URLs de las imágenes en Firebase
                    await _firebaseClient
                        .Child("Product")
                        .Child(newProduct.IdProduct)
                        .PutAsync(newProduct);
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al insertar producto: {ex.Message}");
                return false;
            }
        }


        public async Task<string> UploadProductImageAsync(Stream imageStream, string idProduct)
        {
            try
            {
                var imagePath = $"products/{idProduct}/{Guid.NewGuid()}.jpg";

                // Aumentar el tiempo de espera a 15 minutos
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(15));

                // Comprimir la imagen antes de cargarla
                using (var compressedImageStream = new MemoryStream())
                {
                    await CompressImage(imageStream, compressedImageStream, 80); // Calidad de compresión del 80%
                    compressedImageStream.Position = 0;

                    // Implementar carga por fragmentos
                    const int chunkSize = 1 * 1024 * 1024; // 1 MB por fragmento
                    var buffer = new byte[chunkSize];
                    int bytesRead;
                    long position = 0;

                    while ((bytesRead = await compressedImageStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        var chunk = new MemoryStream(buffer, 0, bytesRead);
                        await _firebaseStorage
                            .Child(imagePath)
                            .PutAsync(chunk, cancellationTokenSource.Token);
                        position += bytesRead;
                    }
                }

                var imageUrl = await _firebaseStorage.Child(imagePath).GetDownloadUrlAsync();

                if (string.IsNullOrEmpty(imageUrl))
                {
                    throw new Exception("No se pudo obtener la URL de la imagen.");
                }

                return imageUrl;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("La carga de la imagen se canceló debido a que excedió el tiempo límite.");
                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al subir imagen: {ex.Message}");
                return string.Empty;
            }
        }
        private async Task CompressImage(Stream input, Stream output, int quality)
        {
            using (var image = await SixLabors.ImageSharp.Image.LoadAsync(input))
            {
                image.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(800, 0), // Ancho máximo de 800px, altura proporcional
                    Mode = ResizeMode.Max
                }));
                await image.SaveAsJpegAsync(output, new JpegEncoder { Quality = quality });
            }
        }


        public override async Task<bool> Update(Product updateProduct, List<Stream> imageStreams)
        {
            try
            {
                var existingProduct = await GetByIdWithCategory(updateProduct.IdProduct);
                if (existingProduct == null)
                {
                    throw new Exception("Producto no encontrado");
                }

                if (existingProduct.ImageUrls != null)
                {
                    foreach (var url in existingProduct.ImageUrls)
                    {
                        if (!updateProduct.ImageUrls.Contains(url))
                        {
                            await DeleteImageFromFirebase(url);
                        }
                    }
                }

                if (imageStreams != null && imageStreams.Any())
                {
                    var newImageUrls = await UploadProductImagesAsync(imageStreams, updateProduct.IdProduct);
                    updateProduct.ImageUrls.AddRange(newImageUrls);
                }

                // Implementar reintentos para la actualización en Firebase
                await RetryHelper.ExecuteWithRetry(async () =>
                {
                    await _firebaseClient
                        .Child("Product")
                        .Child(updateProduct.IdProduct)
                        .PutAsync(updateProduct);
                }, maxRetries: 3);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto: {ex.Message}");
                return false;
            }
        }

        // Método para eliminar imágenes de Firebase Storage
        private async Task DeleteImageFromFirebase(string imageUrl)
        {
            try
            {
                var uri = new Uri(imageUrl);
                int index = uri.AbsolutePath.IndexOf("/o/");
                if (index == -1) return;

                string path = Uri.UnescapeDataString(uri.AbsolutePath.Substring(index + 3));
                if (!string.IsNullOrEmpty(path))
                {
                    await _firebaseStorage.Child(path).DeleteAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar imagen: {ex.Message}");
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

                var imageUrlsData = await _firebaseClient
                    .Child("Product")
                    .Child(id)
                    .Child("ImageUrls")
                    .OnceAsync<string>();

                if (imageUrlsData != null && imageUrlsData.Any())
                {
                    product.ImageUrls = imageUrlsData.Select(i => i.Object).ToList();
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

                // Obtener ImageUrls como una lista de strings
                var imageUrlsData = await _firebaseClient
                    .Child("Product")
                    .Child(p.Key)
                    .Child("ImageUrl")
                    .OnceAsync<string>();

                if (imageUrlsData != null && imageUrlsData.Any())
                {
                    product.ImageUrls = imageUrlsData.Select(i => i.Object).ToList();
                }

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

        public static class RetryHelper
        {
            public static async Task ExecuteWithRetry(Func<Task> action, int maxRetries = 3, int delayMs = 1000)
            {
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        await action();
                        return;
                    }
                    catch (Exception ex)
                    {
                        if (i == maxRetries - 1) throw;
                        Console.WriteLine($"Intento {i + 1} falló: {ex.Message}. Reintentando...");
                        await Task.Delay(delayMs);
                    }
                }
            }



        }
    }
}