using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApp.Models;
using PayrollApp.Service;
using PayrollApp.Services;

namespace PayrollApp.Controllers
{
    [ApiController]
    [Route("payrollapp/users")]
    [Authorize(Roles = "SuperAdmin")]  // ← entire controller is SuperAdmin-only
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController(IUserService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            var result = await _service.CreateUserAsync(request);
            return StatusCode(201, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _service.GetAllUsersAsync();
            return Ok(result);
        }
    }
}