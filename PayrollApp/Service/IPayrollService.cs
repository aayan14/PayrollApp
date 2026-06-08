using PayrollApp.Models;

namespace PayrollApp.Services
{
    public interface IPayrollService
    {
        Task<IEnumerable<PayrollDetail>> RunPayrollAsync(int month, int year);
        Task<IEnumerable<PayrollDetail>> GetByMonthYearAsync(int month, int year);
        Task<PayrollDetail?> GetSlipAsync(int runId, int employeeId);
    }
}