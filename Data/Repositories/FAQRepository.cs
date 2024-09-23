using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;
using home_pisos_vinilicos_admin.Domain.Entities;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class FAQRepository : FirebaseRepository<FAQ>, IFAQRepository
    {
        public FAQRepository() : base()
        {
        }

        public override async Task<bool> Insert(FAQ newFAQ)
        {
            try
            {
                return await base.Insert(newFAQ);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(FAQ updateFAQ)
        {
            try
            {
                return await base.Update(updateFAQ);
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
