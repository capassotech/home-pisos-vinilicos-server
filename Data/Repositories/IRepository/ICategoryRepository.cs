using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories.IRepository
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<List<Category>> GetAll(Expression<Func<Category, bool>>? filter = null);
        Task<Category> GetByIdWithSubCategory(string id);
        Task<List<Category>> GetAllWithSubCategories();
    }
}
