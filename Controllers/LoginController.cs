using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TraineeCoreAPI.DTOs;
using TraineeCoreAPI.Models;

namespace TraineeCoreAPI.Controllers
{

    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly TraineeDbContext _db;
        IConfiguration _configuration;

        public LoginController(TraineeDbContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> PostLoginDetails(UserModel _userData)
        {
            if (_userData != null)
            {
                var loginCheck = _db.Users.Where(x => x.EmailId == _userData.Email && x.Password == _userData.Password).FirstOrDefault();
                if (loginCheck == null)
                {
                    return BadRequest("Invalid crediantials.");
                }
                else
                {
                    _userData.UserMessage = "Login successfull.";
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub,_configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", _userData.Id.ToString()),
                        new Claim("DisplayName", _userData.UserName.ToString()),
                        new Claim("UserName", _userData.Email.ToString()),
                        new Claim("Email", _userData.Email)
                    };
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(30),
                        signingCredentials: signIn
                        );
                    _userData.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
                    return Ok(_userData);
                }
            }
            else
            {
                return BadRequest("No data posted.");
            }
        }
    }
}
