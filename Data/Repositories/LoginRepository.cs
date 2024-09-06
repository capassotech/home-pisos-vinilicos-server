using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Models;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class LoginRepository : FirebaseRepository<Login>, ILoginRepository
    {
        public LoginRepository() : base()
        {
        }

        public override async Task<bool> Insert(Login newLogin)
        {
            try
            {
                return await base.Insert(newLogin);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> Update(Login updateLogin)
        {
            try
            {
                return await base.Update(updateLogin);
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
