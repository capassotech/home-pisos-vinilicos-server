using Firebase.Database;
using Firebase.Database.Query;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class CategoryRepository : FirebaseRepository<Category>, ICategoryRepository
    {
        private readonly FirebaseClient _firebaseClient;

        public CategoryRepository(IConfiguration configuration) : base(configuration)
        {
            var firebaseDatabaseUrl = configuration["Firebase:DatabaseUrl"] ?? Environment.GetEnvironmentVariable("Firebase_DatabaseUrl");

            if (string.IsNullOrEmpty(firebaseDatabaseUrl))
            {
                throw new InvalidOperationException("Firebase:DatabaseUrl no está configurada.");
            }

            _firebaseClient = new FirebaseClient(firebaseDatabaseUrl);
        }

        public override async Task<bool> Insert(Category newCategory, List<Stream>? imageStreams = null)
        {
            try
            {
                if (string.IsNullOrEmpty(newCategory.ParentCategoryId))
                {
                    newCategory.ParentCategoryId = null;
                }
                return await base.Insert(newCategory);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(Category updateCategory, List<Stream>? imageStreams = null)
        {
            try
            {
                if (string.IsNullOrEmpty(updateCategory.ParentCategoryId))
                {
                    updateCategory.ParentCategoryId = null;
                }
                return await base.Update(updateCategory);
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

        public async Task<Category?> GetByIdWithSubCategories(string id)
        {
            var category = await GetCategoryById(id);
            if (category == null)
            {
                return null;
            }

            var subCategories = await GetSubCategoriesByCategoryId(id);
            if (subCategories.Any())
            {
                category.SubCategories = subCategories;
            }

            return category;
        }

        public async Task<Category?> GetCategoryById(string id)
        {
            return await _firebaseClient
                .Child("Category")
                .Child(id)
                .OnceSingleAsync<Category>();
        }

        private async Task<List<Category>> GetSubCategoriesByCategoryId(string idCategory)
        {
            var subCategories = await _firebaseClient
                .Child("Category")
                .OrderBy("ParentCategoryId") 
                .EqualTo(idCategory)
                .OnceAsync<Category>();

            return subCategories.Select(c =>
            {
                var subCategory = c.Object;
                subCategory.IdCategory = c.Key;
                return subCategory;
            }).ToList();
        }

        public async Task<List<Category>> GetAllWithSubCategories()
        {
            try
            {
                var categoryList = await GetAllCategories();

                var subCategoryDictionary = await GetSubCategoryDictionary();

                AssignSubCategoriesToCategories(categoryList, subCategoryDictionary);

                return categoryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                return new List<Category>();
            }
        }

        private async Task<List<Category>> GetAllCategories()
        {
            var categories = await _firebaseClient.Child("Category").OnceAsync<Category>();

            return categories.Select(c =>
            {
                var category = c.Object;
                category.IdCategory = c.Key;
                return category;
            }).ToList();
        }

        private async Task<Dictionary<string, List<Category>>> GetSubCategoryDictionary()
        {
            var subCategories = await _firebaseClient.Child("Category").OnceAsync<Category>();

            return subCategories
                .Where(s => !string.IsNullOrEmpty(s.Object.ParentCategoryId)) 
                .GroupBy(s => s.Object.ParentCategoryId) 
                .ToDictionary(g => g.Key, g => g.Select(c => c.Object).ToList());
        }

        private void AssignSubCategoriesToCategories(List<Category> categories, Dictionary<string, List<Category>> subCategoryDictionary)
        {
            foreach (var category in categories)
            {
                if (subCategoryDictionary.ContainsKey(category.IdCategory))
                {
                    category.SubCategories = subCategoryDictionary[category.IdCategory];
                }
            }
        }
    }
}