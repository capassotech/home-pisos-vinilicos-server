using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class ContactRepository : FirebaseRepository<Contact>, IContactRepository
    {
        public ContactRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<bool> Insert(Contact newContact, List<Stream>? imageStreams = null)
        {
            try
            {
                return await base.Insert(newContact);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(Contact updateContact, List<Stream>? imageStreams = null)
        {
            try
            {
                return await base.Update(updateContact);
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