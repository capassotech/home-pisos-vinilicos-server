using Firebase.Database;
using Firebase.Database.Query;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class ProductRepository : FirebaseRepository<Product>, IProductRepository
    {
        private readonly FirebaseClient _firebaseClient;
        public ProductRepository() : base()
        {
            _firebaseClient = new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/");
        }

        public override async Task<bool> Insert(Product newProduct)
        {
            try
            {
                return await base.Insert(newProduct);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(Product updateProduct)
        {
            try
            {
                return await base.Update(updateProduct);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Delete(string id)
        {
            try
            {
                return await base.Delete(id);
            }
            catch (Exception)
            {
                return false;
            }
        }


        public async Task<Product?> GetByIdWithCategory(string id)
        {
            var product = await _firebaseClient
                .Child("Product")
                .Child(id)
                .OnceSingleAsync<Product>();
            if (product == null)
            {
                return null;
            }
            var category = await _firebaseClient
                .Child("Category")
                .Child(product.IdCategory)
                .OnceSingleAsync<Category>();
            if (category != null)
            {
                product.Category = category; 
            }
            return product;
        }


        public async Task<List<Product>> GetAllWithCategories()
        {
            try
            {
                var products = await _firebaseClient
                    .Child("Product")
                    .OnceAsync<Product>();

                var productList = products.Select(p =>
                {
                    var product = p.Object;
                    product.IdProduct = p.Key;
                    return product;
                }).ToList();
                var categories = await _firebaseClient
                    .Child("Category")
                    .OnceAsync<Category>();
                var categoryDictionary = categories
                    .ToDictionary(c => c.Key, c => c.Object); 
                // Recorre los productos para asignarles su categoría usando el diccionario
                foreach (var product in productList)
                {
                    if (!string.IsNullOrEmpty(product.IdCategory) && categoryDictionary.ContainsKey(product.IdCategory))
                    {
                        product.Category = categoryDictionary[product.IdCategory];
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener productos: {ex.Message}");
                return new List<Product>();
            }
        }




    }
}
