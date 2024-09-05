using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace home_pisos_vinilicos.Application.Services.Firebase
{
    public static class FirebaseInitializer
    {
        public static void InitializeFirebase()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Server", "firebase.json");
            if (File.Exists(path))
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    FirebaseApp.Create(new AppOptions()
                    {
                        Credential = GoogleCredential.FromFile(path)
                    });
                }
            }
            else
            {
                throw new FileNotFoundException("El archivo de configuración de Firebase no se encontró en la ruta especificada.", path);
            }
        }
    }
}
