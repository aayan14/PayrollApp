using PayrollApp.Models;
using PayrollApp.Repos;

namespace PayrollApp.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepo _repo;

        public PayrollService(IPayrollRepo repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<PayrollDetail>> RunPayrollAsync(int month, int year)
        {
            return await _repo.RunPayrollAsync(month, year);
        }

        public async Task<IEnumerable<PayrollDetail>> GetByMonthYearAsync(int month, int year)
        {
            return await _repo.GetByMonthYearAsync(month, year);
        }

        public async Task<PayrollDetail?> GetSlipAsync(int runId, int employeeId)
        {
            return await _repo.GetSlipAsync(runId, employeeId);
        }
    }
}