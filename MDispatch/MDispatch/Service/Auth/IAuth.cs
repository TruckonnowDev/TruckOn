namespace MDispatch.Service.Auth
{
    public interface IAuth
    {
        int Avthorization(string login, string password, ref string description, ref string token);
        int ClearAvt(string token);
        int RequestPasswordChanges(string login, string password, ref string description);
    }
}
