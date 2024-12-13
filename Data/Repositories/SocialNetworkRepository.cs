using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Entities;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class SocialNetworkRepository : FirebaseRepository<SocialNetwork>, ISocialNetworkRepository
    {
        public SocialNetworkRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public override async Task<bool> Insert(SocialNetwork newSocialNetwork, List<Stream>? imageStreams = null)
        {
            try
            {
                return await base.Insert(newSocialNetwork);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(SocialNetwork updateSocialNetwork, List<Stream>? imageStreams = null)
        {
            try
            {
                return await base.Update(updateSocialNetwork);
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
