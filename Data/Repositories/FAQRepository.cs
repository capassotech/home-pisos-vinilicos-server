using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class FAQRepository : FirebaseRepository<FAQ>, IFAQRepository
    {
        public FAQRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<bool> Insert(FAQ newFAQ, List<Stream>? imageStreams = null)
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

        public override async Task<bool> Update(FAQ updateFAQ, List<Stream>? imageStreams = null)
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