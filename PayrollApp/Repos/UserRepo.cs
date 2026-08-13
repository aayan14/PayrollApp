using Dapper;
using PayrollApp.Models;
using System.Data;

namespace PayrollApp.Repos
{
    public class UserRepo : IUserRepo
    {
        private readonly IDbConnection _db;

        public UserRepo(IDbConnection db)
        {
            _db = db;
        }

        

        public async Task<User?> GetByUsernameAsync(string username)
        {
            const string sql = @"
                SELECT UserId, Username, PasswordHash, FullName, Role, IsActive
                FROM Users
                WHERE Username = @Username AND IsActive = 1";

            return await _db.QueryFirstOrDefaultAsync<User>(sql, new { Username = username });
        }

        public async Task<int> CreateUserAsync(User user)
        {
            const string sql = @"
                INSERT INTO Users (Username, PasswordHash, FullName, Role, IsActive)
                VALUES (@Username, @PasswordHash, @FullName, @Role, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            return await _db.QuerySingleAsync<int>(sql, user);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            const string sql = "SELECT UserId, Username, FullName, Role, IsActive FROM Users";
            return await _db.QueryAsync<User>(sql);
        }

    }
}
