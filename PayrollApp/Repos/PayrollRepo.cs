using Dapper;
using System.Data;
using PayrollApp.Models;

namespace PayrollApp.Repos
{
    public class PayrollRepo : IPayrollRepo
    {
        private readonly IDbConnection _db;

        public PayrollRepo(IDbConnection db)
        {
            _db = db;
        }

        public async Task<IEnumerable<PayrollDetail>> RunPayrollAsync(int month, int year)
        {
            return await _db.QueryAsync<PayrollDetail>("usp_RunPayroll",
                new { Month = month, Year = year },
                commandType: CommandType.StoredProcedure

            );
        }

        public async Task<IEnumerable<PayrollDetail>> GetByMonthYearAsync(int month, int year)
        {
            return await _db.QueryAsync<PayrollDetail>(
                "usp_GetPayrollByMonthYear",
                new { Month = month, Year = year }
            );
        }

        public async Task<PayrollDetail?> GetSlipAsync(int runId, int employeeId)
        {
            return await _db.QueryFirstOrDefaultAsync<PayrollDetail>(
                "usp_GetPayrollSlip",
                new { RunId = runId, EmployeeId = employeeId }
            );
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await _db.QueryAsync<Employee>(
                "usp_GetAllEmployees",
                commandType: CommandType.StoredProcedure
            );
        }
    }
}