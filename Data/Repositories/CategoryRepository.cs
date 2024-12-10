using Firebase.Database;
using Firebase.Database.Query;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class CategoryRepository : FirebaseRepository<Category>, ICategoryRepository
    {
        private readonly FirebaseClient _firebaseClient;

        public CategoryRepository() : base()
        {
            _firebaseClient = new FirebaseClient("https://hpv-desarrollo-default-rtdb.firebaseio.com");
        }

        public override async Task<bool> Insert(Category newCategory, List<Stream>? imageStreams = null)
        {
            try
            {
                // Asigna el ParentCategoryId si es necesario
                if (string.IsNullOrEmpty(newCategory.ParentCategoryId))
                {
                    newCategory.ParentCategoryId = null; // o alguna lógica para establecerlo
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
                // Asegúrate de que el ParentCategoryId esté presente
                if (string.IsNullOrEmpty(updateCategory.ParentCategoryId))
                {
                    updateCategory.ParentCategoryId = null; // o alguna lógica para establecerlo
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

            // Obtener subcategorías asociadas a la categoría
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
                .OrderBy("ParentCategoryId") // Cambiado a "ParentCategoryId"
                .EqualTo(idCategory) // Filtrar por el ParentCategoryId
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

                // Obtener todas las subcategorías de una vez
                var subCategoryDictionary = await GetSubCategoryDictionary();

                // Asignar subcategorías a cada categoría
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
