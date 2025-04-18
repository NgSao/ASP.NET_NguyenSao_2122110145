using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Email và mật khẩu là bắt buộc.");
            }

            var user = _context.Users
                .FirstOrDefault(u => u.Email == model.Email);

            if (user == null || !VerifyPassword(model.Password, user.PasswordHash))
            {
                return Unauthorized("Email hoặc mật khẩu không đúng.");
            }

            var token = GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string password, string? passwordHash)
        {
            if (string.IsNullOrEmpty(passwordHash))
            {
                return false;
            }

            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Email và mật khẩu là bắt buộc.");
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return Conflict("Email đã được sử dụng.");
            }

            var newUser = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return Ok("Đăng ký thành công.");
        }




    }
}

public class LoginDto
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string Name { get; set; }

    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
    public RoleType Role { get; set; } = RoleType.Customer;
    public UserStatus Status { get; set; } = UserStatus.Active;
}

