using PayrollApp.Models;

namespace PayrollApp.Repos
{
    public interface IPayrollRepo
    {
        Task<IEnumerable<PayrollDetail>> RunPayrollAsync(int month, int year);
        Task<IEnumerable<PayrollDetail>> GetByMonthYearAsync(int month, int year);
        Task<PayrollDetail?> GetSlipAsync(int runId, int employeeId);

        Task<IEnumerable<PayrollAttendanceDto>> GetAttendanceForPayrollAsync(int month, int year);
        Task<int> CreatePayrollRunAsync(int month, int year);
        Task SavePayrollDetailsAsync(IEnumerable<PayrollDetail> details);
    }
}