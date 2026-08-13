using Microsoft.AspNetCore.Mvc;
using PayrollApp.Models;
using PayrollApp.Service;


namespace PayrollApp.Controllers
{
    [ApiController]
    [Route("payrollapp/auth")]
    public class AuthController: ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.Login(request.Username, request.Password);
            if (result == null)
            {

                return Unauthorized(new { message = "Invalid username or password." });
            }

            return Ok(result);
        }
    }
}
