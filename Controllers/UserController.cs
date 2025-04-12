using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra email đã tồn tại
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (existingUser != null)
            {
                return Conflict("Email đã được sử dụng.");
            }

            var newUser = new User
            {
                Name = model.Name,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Avatar = model.Avatar,
                Role = model.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            // Trả về thông tin người dùng mới (không bao gồm PasswordHash)
            var userResponse = new
            {
                newUser.Id,
                newUser.Name,
                newUser.Email,
                newUser.Avatar,
                newUser.Role,
                newUser.CreatedDate
            };

            return CreatedAtAction(nameof(GetUser), new { id = newUser.Id }, userResponse);
        }

        // GET: api/user
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Avatar,
                    u.Role,
                    u.CreatedDate,
                    u.UpdatedDate
                })
                .ToListAsync();
            return Ok(users);
        }

        // GET: api/user/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,User")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Avatar,
                    u.Role,
                    u.CreatedDate,
                    u.UpdatedDate
                })
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Người dùng chỉ được xem thông tin của chính họ
            if (User.IsInRole(nameof(RoleType.User)))
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (currentUserId != id)
                {
                    return Forbid();
                }
            }

            return Ok(user);
        }

        // GET: api/user/profile
        [HttpGet("profile")]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.Avatar,
                    u.Role,
                    u.CreatedDate,
                    u.UpdatedDate
                })
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            return Ok(user);
        }

        // PUT: api/user/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Người dùng chỉ được cập nhật thông tin của chính họ
            if (User.IsInRole(nameof(RoleType.User)))
            {
                var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (currentUserId != id)
                {
                    return Forbid();
                }
            }

            // Chỉ Admin được phép cập nhật vai trò
            if (model.Role.HasValue && !User.IsInRole(nameof(RoleType.Admin)))
            {
                return Forbid("Chỉ Admin mới có thể thay đổi vai trò.");
            }

            user.Name = model.Name ?? user.Name;
            user.Avatar = model.Avatar ?? user.Avatar;
            if (!string.IsNullOrEmpty(model.Email) && model.Email != user.Email)
            {
                // Kiểm tra email mới có trùng không
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    return Conflict("Email đã được sử dụng.");
                }
                user.Email = model.Email;
            }
            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }
            if (model.Role.HasValue)
            {
                user.Role = model.Role.Value;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/user/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound("Người dùng không tồn tại.");
            }

            // Ngăn xóa tài khoản của chính mình
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (currentUserId == id)
            {
                return BadRequest("Không thể xóa tài khoản của chính bạn.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }

    // Model cho cập nhật người dùng
    public class UpdateUserModel
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Avatar { get; set; }
        public RoleType? Role { get; set; }
    }

    public class CreateUserModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required, EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        public string? Avatar { get; set; }
        [Required]
        public RoleType Role { get; set; }
    }
}