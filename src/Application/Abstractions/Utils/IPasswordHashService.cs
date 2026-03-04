namespace Application.Abstractions.Utils
{
    public interface IPasswordHashService
    {
        string PasswordHashing(string password);
        bool VerifyPassword(string password, string storedFullHash);
    }
}
