using PayrollApp.Models;

namespace PayrollApp.Service
{
    public interface IUserService
    {
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
    }
}
