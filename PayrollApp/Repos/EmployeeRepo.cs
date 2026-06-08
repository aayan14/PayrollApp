using Dapper;
using System.Data;
using PayrollApp.Models;

namespace PayrollApp.Repos
{
    public class EmployeeRepo : IEmployeeRepo
    {
        private readonly IDbConnection _db;

        public EmployeeRepo(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.QueryAsync<Employee>("SELECT * FROM EMPLOYEES");
        }
    }

}