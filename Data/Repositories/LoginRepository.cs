using Firebase.Database;
using Firebase.Database.Query;  // Asegúrate de tener esta referencia
using home_pisos_vinilicos.Data.Repositories.IRepository;
using home_pisos_vinilicos.Domain.Models;
using System.Linq.Expressions;

namespace home_pisos_vinilicos.Data.Repositories
{
    public class LoginRepository : FirebaseRepository<Login>, ILoginRepository
    {
        private readonly FirebaseClient _firebaseClient;

        // Constructor que inicializa el FirebaseClient sin autenticación
        public LoginRepository() : base()
        {
            _firebaseClient = new FirebaseClient("https://home-pisos-vinilicos-default-rtdb.firebaseio.com/");
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

        // Método para obtener todos los logins, con o sin filtro
        public async Task<IEnumerable<Login>> GetAll(Expression<Func<Login, bool>>? filter = null)
        {
            // Recupera los usuarios desde Firebase Realtime Database
            var users = await _firebaseClient
                .Child("logins")
                .OnceAsync<Login>();

            // Convierte los datos recuperados en una lista de Login
            var logins = users.Select(u => u.Object).AsQueryable();

            // Si se proporciona un filtro, lo aplicamos
            if (filter != null)
            {
                logins = logins.Where(filter);
            }

            return logins.ToList();
        }
    }
}
