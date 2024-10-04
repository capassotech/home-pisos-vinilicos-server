namespace home_pisos_vinilicos.Application.Interfaces
{
    public interface ITokenService
    {
        string GetToken();
        void SetToken(string token);
    }
}
