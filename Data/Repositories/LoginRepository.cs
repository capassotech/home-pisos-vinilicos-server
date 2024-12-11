using Firebase.Database;
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Models;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class LoginRepository : FirebaseRepository<Login>, ILoginRepository
    {
        private readonly FirebaseClient _firebaseClient;

        public LoginRepository(IConfiguration configuration) : base(configuration)
        {
            var firebaseDatabaseUrl = configuration["Firebase:DatabaseUrl"] ?? Environment.GetEnvironmentVariable("Firebase_DatabaseUrl");

            if (string.IsNullOrEmpty(firebaseDatabaseUrl))
            {
                throw new InvalidOperationException("Firebase:DatabaseUrl no está configurada.");
            }

            _firebaseClient = new FirebaseClient(firebaseDatabaseUrl);
        }

        public override async Task<bool> Insert(Login newLogin, List<Stream>? imageStreams = null)
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

        public override async Task<bool> Update(Login updateLogin, List<Stream>? imageStreams = null)
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

        public async Task<IEnumerable<Login>> GetAll(Expression<Func<Login, bool>>? filter = null)
        {
            var users = await _firebaseClient
                .Child("logins")
                .OnceAsync<Login>();

            var logins = users.Select(u => u.Object).AsQueryable();

            if (filter != null)
            {
                logins = logins.Where(filter);
            }

            return logins.ToList();
        }
    }
}