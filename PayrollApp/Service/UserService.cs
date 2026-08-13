using PayrollApp.Models;
using PayrollApp.Repos;
using System.Data;

namespace PayrollApp.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _repo;

        public UserService(IUserRepo repo)
        {
            _repo = repo;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var exists = await _repo.GetByUsernameAsync(request.Username);

            if (exists != null) 
            {
                throw new InvalidOperationException("Username already exists!");
            }

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                IsActive = true,
                Role = request.Role,
            };

            int addedUserId = await _repo.CreateUserAsync(user);

            return new UserResponse
            {
                UserId = addedUserId,
                Username = request.Username,
                FullName = request.FullName,
                IsActive = true,
                Role = request.Role,
            };

        }

        public async  Task<IEnumerable<UserResponse>> GetAllUsersAsync()
        {
            var users = await _repo.GetAllAsync();
            return users.Select(u => new UserResponse
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Role = u.Role,
                IsActive = u.IsActive
            });
        }
    }
}
