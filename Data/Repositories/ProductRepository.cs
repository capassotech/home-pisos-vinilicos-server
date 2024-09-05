using home_pisos_vinilicos.Data.Repositories;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos_admin.Domain;

namespace home_pisos_vinilicos.Data
{
    public class ProductRepository : FirebaseRepository<Product>, IProductRepository
    {
        public ProductRepository() : base()
        {
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
    }
}
