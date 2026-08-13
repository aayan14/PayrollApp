using PayrollApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PayrollApp.Repos;
using System.Threading.Tasks;

namespace PayrollApp.Service
{
    public class AuthService : IAuthService
    {

        private readonly IConfiguration _config;
        private readonly IUserRepo _userrepo;

        public AuthService(IConfiguration config, IUserRepo userrepo)
        {
            _config = config;
            _userrepo = userrepo;
        }



        public async Task<LoginResponse?> Login(string username, string password)
        {

            var user = await _userrepo.GetByUsernameAsync(username);
            if (user == null)
            {
                return null;
            }   

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }



            var expiryMinutes = int.Parse(_config["JWT:ExpiryMinutes"]);
            var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

            var claim = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Name, user.FullName)
                
            };

            var key  = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["JWT:Issuer"],
                audience: _config["JWT:Audience"],
                claims: claim,
                expires: expiresAt,
                signingCredentials: creds
                );

            return new LoginResponse
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiry = expiresAt
            };

            

        }
    }
}
