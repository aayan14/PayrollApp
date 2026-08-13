using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApp.Models;
using PayrollApp.Services;

namespace PayrollApp.Controllers
{
    [ApiController]
    [Route("payrollapp/payroll")]
    [Authorize]
    public class PayrollApp : ControllerBase
    {
        private readonly IPayrollService _service;
        private readonly ILogger<PayrollApp> _logger;

        public PayrollApp(IPayrollService service, ILogger<PayrollApp> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("run")]
        [Authorize (Roles="SuperAdmin")]
        public async Task<IActionResult> RunPayroll([FromBody] PayrollRunRequest request)
        {
            
                var result = await _service.RunPayrollAsync(request.Month, request.Year);
            _logger.LogInformation("RunPayroll Controller Ended");
                return StatusCode(201, result);
            

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