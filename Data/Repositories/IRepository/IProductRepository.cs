using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<List<Product>> GetAll(Expression<Func<Product, bool>>? filter = null);
        Task<Product> GetByIdWithCategory(string id);
        Task<List<Product>> GetAllWithCategories();
        Task<string> UploadProductImageAsync(Stream imageStream, string idProduct);
        Task<bool> Insert(Product newProduct, List<Stream> imageStreams);
        Task<bool> Update(Product updateProduct, List<Stream> imageStreams);
        Task<bool> UpdateRangeAsync(IEnumerable<Product> products);
    }
}
