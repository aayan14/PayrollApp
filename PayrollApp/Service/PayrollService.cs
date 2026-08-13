using PayrollApp.Models;
using PayrollApp.Repos;
using PayrollApp.Service;

namespace PayrollApp.Services
{
    public class PayrollService : IPayrollService
    {
        private readonly IPayrollRepo _repo;
        private readonly ILogger<PayrollService> _logger;

        public PayrollService(IPayrollRepo repo, ILogger<PayrollService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<IEnumerable<PayrollDetail>> RunPayrollAsync(int month, int year)
        {
            _logger.LogInformation($"Starting payroll run for {month}/{year}");

            var attendanceData = await _repo.GetAttendanceForPayrollAsync(month, year);

            // 2. Create the run (throws if duplicate — caught by middleware)
            var runId = await _repo.CreatePayrollRunAsync(month, year);

            // 3. Calculate using PayrollCalculator (already unit tested!)
            var details = attendanceData.Select(a =>
            {
                var gross = PayrollCalculator.CalculateGrossPay(a.BasicSalary, a.TotalWorkingDays, a.DaysPresent);
                var pf = PayrollCalculator.CalculatePF(a.BasicSalary);
                var net = PayrollCalculator.CalculateNetPay(gross, pf);

                return new PayrollDetail
                {
                    RunId = runId,
                    EmployeeId = a.EmployeeId,
                    Name = a.Name,
                    BasicSalary = a.BasicSalary,
                    TotalWorkingDays = a.TotalWorkingDays,
                    DaysPresent = a.DaysPresent,
                    GrossPay = gross,
                    PFDeduction = pf,
                    ProfessionalTax = 200m,
                    NetPay = net
                };
            }).ToList();

            // 4. Save calculated results
            await _repo.SavePayrollDetailsAsync(details);

            _logger.LogInformation("Payroll run completed for {Month}/{Year}. {EmployeeCount} employees processed",
                month, year, details.Count());

            return details;
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