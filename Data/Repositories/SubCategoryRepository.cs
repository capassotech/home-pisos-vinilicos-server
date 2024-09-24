using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class SubCategoryRepository : FirebaseRepository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository() : base()
        {
        }

        public override async Task<bool> Insert(SubCategory newSubCategory)
        {
            try
            {
                return await base.Insert(newSubCategory);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(SubCategory updateSubCategory)
        {
            try
            {
                return await base.Update(updateSubCategory);
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
