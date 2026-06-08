using Microsoft.AspNetCore.Mvc;
using PayrollApp.Models;
using PayrollApp.Services;

namespace PayrollApp.Controllers
{
    [ApiController]
    [Route("payrollapp/payroll")]
    public class PayrollApp : ControllerBase
    {
        private readonly IPayrollService _service;

        public PayrollApp(IPayrollService service)
        {
            _service = service;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunPayroll([FromBody] PayrollRunRequest request)
        {
            try
            {
                var result = await _service.RunPayrollAsync(request.Month, request.Year);
                return StatusCode(201, result);
            }
            catch (Exception ex) 
            {
                return Conflict("Payroll run already exists for this month and year.");
            }

        }

        [HttpGet("run/{month}/{year}")]
        public async Task<IActionResult> GetByMonthYear(int month, int year)
        {
            var result = await _service.GetByMonthYearAsync(month, year);
            if (!result.Any()) return NotFound();
            return Ok(result);
        }

        [HttpGet("{runId}/slip/{employeeId}")]
        public async Task<IActionResult> GetSlip(int runId, int employeeId)
        {
            var result = await _service.GetSlipAsync(runId, employeeId);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}