using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAll(Expression<Func<T, bool>>? filter = null);
        Task<T> GetById(string id); 
        Task<bool> Insert(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(string id);
    }
}
