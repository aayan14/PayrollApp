using PayrollApp.Models;

namespace PayrollApp.Repos
{
    public interface IUserRepo
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<int> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
