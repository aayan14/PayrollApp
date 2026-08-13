using PayrollApp.Models;

namespace PayrollApp.Service
{
    public interface IAuthService
    {
        Task<LoginResponse?> Login(string username, string password);
    }
}
