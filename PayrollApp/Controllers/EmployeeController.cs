using Microsoft.AspNetCore.Mvc;
using PayrollApp.Models;
using PayrollApp.Repos;

namespace PayrollApp.Controllers
{
    [ApiController]
    [Route("payrollapp/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeRepo _employeeRepo;

        public EmployeeController(IEmployeeRepo repo) {
            _employeeRepo = repo;
        }

        [HttpGet]
        public async Task<IEnumerable<Employee>> GetAll()
        {
            return await _employeeRepo.GetAllAsync();
        }
    }
}