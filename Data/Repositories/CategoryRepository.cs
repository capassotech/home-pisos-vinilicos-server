using Firebase.Database;
using Firebase.Database.Query;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class CategoryRepository : FirebaseRepository<Category>, ICategoryRepository
    {
        private readonly FirebaseClient _firebaseClient;
        public CategoryRepository() : base()
        {
            _firebaseClient = new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/");
        }

        public override async Task<bool> Insert(Category newCategory)
        {
            try
            {
                return await base.Insert(newCategory);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(Category updateCategory)
        {
            try
            {
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

        public async Task<Category?> GetByIdWithSubCategory(string id)
        {
            var category = await GetCategoryById(id);
            if (category == null)
            {
                return null;
            }

            // Obtener la subcategoría asociada a la categoría
            var subCategory = await GetSubCategoryById(category.IdSubCategory);
            if (subCategory != null)
            {
                category.SubCategory = subCategory; // Asignar la subcategoría
            }

            return category;
        }

        private async Task<Category?> GetCategoryById(string id)
        {
            return await _firebaseClient
                .Child("Category")
                .Child(id)
                .OnceSingleAsync<Category>();
        }

        private async Task<SubCategory?> GetSubCategoryById(string? idSubCategory)
        {
            if (string.IsNullOrEmpty(idSubCategory))
            {
                return null;
            }

            return await _firebaseClient
                .Child("SubCategory")
                .Child(idSubCategory)
                .OnceSingleAsync<SubCategory>();
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

        private async Task<Dictionary<string, SubCategory>> GetSubCategoryDictionary()
        {
            var subCategories = await _firebaseClient.Child("SubCategory").OnceAsync<SubCategory>();
            return subCategories.ToDictionary(s => s.Key, s => s.Object);
        }

        private void AssignSubCategoriesToCategories(List<Category> categories, Dictionary<string, SubCategory> subCategoryDictionary)
        {
            foreach (var category in categories)
            {
                if (!string.IsNullOrEmpty(category.IdSubCategory) && subCategoryDictionary.ContainsKey(category.IdSubCategory))
                {
                    category.SubCategory = subCategoryDictionary[category.IdSubCategory];
                }
            }
        }
    }

}