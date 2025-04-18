using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;
using System.Security.Claims;

namespace NguyenSao_2122110145.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public UsersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        // [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            var userDtos = _mapper.Map<List<UserResponseDto>>(users);
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound();

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
                return BadRequest("Email đã được sử dụng.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = _mapper.Map<User>(userDto);
            user.UserStatus = UserStatus.Active;
            user.Password = hashedPassword;
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            var responseDto = _mapper.Map<UserResponseDto>(user);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, responseDto);
        }


        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAdminUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
                return BadRequest("Email đã được sử dụng.");

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            var userResponseDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userResponseDto);
        }



        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("email/{email}")]
        [Authorize]
        public async Task<IActionResult> GetUserByEmail(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userClaim = User.Claims
               .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _))
               .FirstOrDefault();

            if (userClaim == null)
            {
                return Unauthorized("Không tìm thấy claim định danh người dùng hợp lệ.");
            }


            if (!int.TryParse(userClaim.Value, out var userId))
            {
                return BadRequest("ID người dùng không hợp lệ.");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            var userDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userDto);
        }





    }
}