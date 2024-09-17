using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class CategoryRepository : FirebaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository() : base()
        {
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



    }
}
