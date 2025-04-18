using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;
using NguyenSao_2122110145.Services;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public AuthController(AppDbContext context, IConfiguration configuration, IEmailService emailService, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
            _mapper = mapper;


        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password))
            {
                return BadRequest(new { message = "Email và mật khẩu là bắt buộc." });

            }

            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null || !VerifyPassword(model.Password, user.Password))
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });
            }
            if (user.UserStatus == UserStatus.Inactive)
                return BadRequest(new { message = "Tài khoản chưa được kích hoạt. Vui lòng kích hoạt" });


            if (user.UserStatus == UserStatus.Blocked)
                return BadRequest(new { message = "Tài khoản bị đã khóa. Vui lòng liên hệ lại cửa hàng." });

            user.LastLogin = DateTime.UtcNow;
            _context.SaveChanges();

            var token = GenerateJwtToken(user);
            return Ok(new
            {
                token = token,
                user = new
                {
                    fullName = user.FullName,
                    avatar = user.Avatar
                }
            });
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

            Console.WriteLine("[DEBUG] Claims in GenerateJwtToken:");
            foreach (var claim in claims)
            {
                Console.WriteLine($"[DEBUG] Claim Type: {claim.Type}, Value: {claim.Value}");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Jwt:ExpireMinutes"]!)),
                signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            Console.WriteLine($"[DEBUG] Generated Token: {tokenString}");

            return tokenString;
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
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "Email và mật khẩu là bắt buộc." });
            }

            var existingUser = _context.Users.FirstOrDefault(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                return Conflict(new { message = "Email đã được sử dụng." });
            }
            var newUser = new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = RoleStatus.Customer,
                UserStatus = UserStatus.Inactive
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntry = new Otp
            {
                Email = dto.Email,
                Code = otp,
                Attempts = 0,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };
            _context.Otps.Add(otpEntry);
            await _context.SaveChangesAsync();
            var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5000";
            var verificationLink = $"{baseUrl}/api/auth/verify-email?email={Uri.EscapeDataString(dto.Email)}&code={otp}";

            // Soạn nội dung email
            var emailBody = $@"
            <p>Xin chào {dto.FullName},</p>
            <p>Đây là mã xác thực của bạn: <strong>{otp}</strong></p>
            <p>Hoặc bạn có thể nhấn vào liên kết sau để xác thực tài khoản:</p>
            <p><a href=""{verificationLink}"">Xác thực tài khoản</a></p>
            <p>Mã này sẽ hết hạn sau 3 phút.</p> ";

            await _emailService.SendEmailAsync(dto.Email, "Mã xác thực tài khoản", emailBody);
            return Ok(new { message = "Mã xác thực đã được gửi đến email của bạn." });
        }


        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyOtp([FromBody] OtpVerifyDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Otp))
                return BadRequest(new { message = "Email và mã OTP là bắt buộc." });

            var otpEntry = await _context.Otps
                .FirstOrDefaultAsync(o => o.Email == dto.Email && o.ExpiresAt > DateTime.UtcNow);

            if (otpEntry == null)
                return BadRequest(new { message = "OTP đã hết hạn hoặc không tồn tại. Vui lòng yêu cầu lại." });

            if (dto.Otp != otpEntry.Code)
            {
                otpEntry.Attempts++;
                if (otpEntry.Attempts >= 3)
                {
                    _context.Otps.Remove(otpEntry);
                    await _context.SaveChangesAsync();
                    return BadRequest(new { message = "Bạn đã nhập sai quá 3 lần. Vui lòng yêu cầu lại." });
                }

                await _context.SaveChangesAsync();
                return BadRequest(new { message = $"Mã OTP không đúng. Bạn còn {3 - otpEntry.Attempts} lần thử." });
            }

            // OTP đúng → cập nhật trạng thái
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            user.UserStatus = UserStatus.Active;
            _context.Otps.Remove(otpEntry);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Xác thực thành công. Tài khoản đã được kích hoạt." });
        }


        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest(new { message = "Email là bắt buộc." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            if (user.UserStatus == UserStatus.Blocked)
                return BadRequest(new { message = "Tài khoản bị đã khóa. Vui lòng liên hệ lại cửa hàng." });

            // Xóa OTP cũ (nếu có)
            var existingOtps = _context.Otps.Where(o => o.Email == dto.Email);
            _context.Otps.RemoveRange(existingOtps);

            // Tạo và lưu OTP mới
            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntry = new Otp
            {
                Email = dto.Email,
                Code = otp,
                Attempts = 0,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };

            _context.Otps.Add(otpEntry);
            await _context.SaveChangesAsync();

            var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5000";
            var verificationLink = $"{baseUrl}/api/auth/verify-email?email={Uri.EscapeDataString(dto.Email)}&code={otp}";

            var emailBody = $@"
                <p>Xin chào {user.FullName},</p>
                <p>Đây là mã xác thực mới của bạn: <strong>{otp}</strong></p>
                <p>Hoặc bạn có thể nhấn vào liên kết sau để xác thực tài khoản:</p>
                <p><a href=""{verificationLink}"">Xác thực tài khoản</a></p>
                <p>Mã này sẽ hết hạn sau 3 phút.</p>";

            await _emailService.SendEmailAsync(dto.Email, "Mã xác thực mới", emailBody);
            return Ok(new { message = "Mã OTP mới đã được gửi đến email của bạn." });
        }

        [HttpPost("verify-reset-pass")]
        public async Task<IActionResult> VerifyResetPassword([FromBody] OtpVerifyDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Otp))
                return BadRequest(new { message = "Email và mã OTP là bắt buộc." });

            var otpEntry = await _context.Otps
                .FirstOrDefaultAsync(o => o.Email == dto.Email && o.ExpiresAt > DateTime.UtcNow);

            if (otpEntry == null)
                return BadRequest(new { message = "OTP đã hết hạn hoặc không tồn tại. Vui lòng yêu cầu lại." });

            if (dto.Otp != otpEntry.Code)
            {
                otpEntry.Attempts++;

                if (otpEntry.Attempts >= 3)
                {
                    _context.Otps.Remove(otpEntry);
                    await _context.SaveChangesAsync();
                    return BadRequest(new { message = "Bạn đã nhập sai quá 3 lần. Vui lòng yêu cầu lại." });
                }

                await _context.SaveChangesAsync();
                return BadRequest(new { message = $"Mã OTP không đúng. Bạn còn {3 - otpEntry.Attempts} lần thử." });
            }

            var newPassword = GenerateRandomPassword();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            user.Password = hashedPassword;
            _context.Otps.Remove(otpEntry); // Xóa OTP sau khi đặt lại mật khẩu
            await _context.SaveChangesAsync();

            var emailBody = $@"
                <p>Xin chào {user.FullName},</p>
                <p>Đây là mật khẩu mới của bạn: <strong>{newPassword}</strong></p>
                <p>Vui lòng thay đổi mật khẩu của bạn ngay sau khi đăng nhập.</p>";

            await _emailService.SendEmailAsync(user.Email, "Mật khẩu mới của bạn", emailBody);
            return Ok(new { message = "Mật khẩu của bạn đã được thay đổi và gửi đến email." });
        }

        private string GenerateRandomPassword()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()";
            var password = new string(Enumerable.Range(0, 8)
                                              .Select(_ => chars[random.Next(chars.Length)])
                                              .ToArray());
            return password;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest(new { message = "Email là bắt buộc." });

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
                return NotFound(new { message = "Không tìm thấy người dùng." });

            if (user.UserStatus == UserStatus.Blocked)
                return BadRequest(new { message = "Tài khoản bị khóa. Vui lòng liên hệ cửa hàng." });

            // Xóa OTP cũ (nếu có)
            var existingOtps = _context.Otps.Where(o => o.Email == dto.Email);
            _context.Otps.RemoveRange(existingOtps);

            // Tạo và lưu OTP mới
            var otp = new Random().Next(100000, 999999).ToString();
            var otpEntry = new Otp
            {
                Email = dto.Email,
                Code = otp,
                Attempts = 0,
                ExpiresAt = DateTime.UtcNow.AddMinutes(3)
            };

            _context.Otps.Add(otpEntry);
            await _context.SaveChangesAsync();

            var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:5000";
            var verificationLink = $"{baseUrl}/api/auth/verify-reset-pass?email={Uri.EscapeDataString(dto.Email)}&code={otp}";

            var emailBody = $@"
                <p>Xin chào {user.FullName},</p>
                <p>Đây là mã OTP để đặt lại mật khẩu của bạn: <strong>{otp}</strong></p>
                <p>Hoặc bạn có thể nhấn vào liên kết sau để đặt lại mật khẩu:</p>
                <p><a href=""{verificationLink}"">Đặt lại mật khẩu</a></p>
                <p>Mã này sẽ hết hạn sau 3 phút.</p>";

            await _emailService.SendEmailAsync(dto.Email, "Đặt lại mật khẩu", emailBody);
            return Ok(new { message = "Mã OTP đã được gửi đến email của bạn." });
        }

        // Controllers/AuthController.cs
        [HttpPut("me/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDto userDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Get the current user's ID from the token
            var userClaim = User.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && int.TryParse(c.Value, out _));

            if (userClaim == null)
                return Unauthorized("Không tìm thấy claim định danh người dùng hợp lệ.");

            if (!int.TryParse(userClaim.Value, out var currentUserId))
                return BadRequest("ID người dùng không hợp lệ.");

            // Check if the user is authorized to update this user's information
            if (currentUserId != id && !User.IsInRole("Admin"))
                return Forbid("Bạn không có quyền cập nhật thông tin người dùng này.");

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("Người dùng không tồn tại.");

            // Check if the new email is already taken by another user
            if (!string.IsNullOrEmpty(userDto.Email) && await _context.Users.AnyAsync(u => u.Email == userDto.Email && u.Id != id))
                return BadRequest("Email đã được sử dụng.");

            // Update user details
            bool isPasswordUpdated = false;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                if (string.IsNullOrEmpty(userDto.OldPassword) || !BCrypt.Net.BCrypt.Verify(userDto.OldPassword, user.Password))
                    return BadRequest("Mật khẩu cũ không đúng.");

                if (userDto.Password.Length < 8)
                    return BadRequest("Mật khẩu mới phải có ít nhất 8 ký tự.");

                user.Password = BCrypt.Net.BCrypt.HashPassword(userDto.Password);
                isPasswordUpdated = true;
            }

            // Update other fields if provided
            if (!string.IsNullOrEmpty(userDto.FullName))
                user.FullName = userDto.FullName;
            if (!string.IsNullOrEmpty(userDto.Email))
                user.Email = userDto.Email;

            // Handle avatar upload (base64)
            if (!string.IsNullOrEmpty(userDto.Avatar))
            {
                try
                {
                    // Extract base64 data (remove "data:image/...;base64," prefix)
                    var base64String = userDto.Avatar;
                    if (base64String.Contains(","))
                    {
                        base64String = base64String.Split(',')[1];
                    }

                    var bytes = Convert.FromBase64String(base64String);

                    // Validate file type
                    var extension = ".jpg"; // Mặc định
                    if (userDto.Avatar.StartsWith("data:image/png"))
                        extension = ".png";
                    else if (userDto.Avatar.StartsWith("data:image/jpeg"))
                        extension = ".jpeg";
                    else
                        return BadRequest("Chỉ hỗ trợ định dạng ảnh JPG, JPEG, PNG.");

                    // Validate size (5MB)
                    if (bytes.Length > 5 * 1024 * 1024)
                        return BadRequest("Kích thước ảnh không được vượt quá 5MB.");

                    // Generate unique file name
                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars", fileName);

                    // Ensure directory exists
                    Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/avatars"));

                    // Save file
                    await System.IO.File.WriteAllBytesAsync(filePath, bytes);

                    // Delete old avatar if exists
                    if (!string.IsNullOrEmpty(user.Avatar))
                    {
                        var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.Avatar.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePath))
                            System.IO.File.Delete(oldFilePath);
                    }

                    // Update avatar URL
                    user.Avatar = $"/avatars/{fileName}";
                }
                catch (FormatException)
                {
                    return BadRequest("Dữ liệu base64 không hợp lệ.");
                }
            }

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // Send email notification
            var emailBody = $@"
        <p>Xin chào {user.FullName},</p>
        <p>Thông tin tài khoản của bạn đã được cập nhật thành công.</p>";

            if (isPasswordUpdated)
            {
                emailBody += "<p>Mật khẩu của bạn đã được thay đổi. Vui lòng đăng nhập bằng mật khẩu mới.</p>";
            }
            else
            {
                emailBody += $@"
            <p><strong>Thông tin mới:</strong></p>
            <ul>
                <li>Họ và tên: {user.FullName}</li>
                <li>Email: {user.Email}</li>
                {(userDto.Avatar != null ? "<li>Ảnh đại diện: Đã cập nhật</li>" : "")}
            </ul>";
            }

            emailBody += "<p>Nếu bạn không thực hiện thay đổi này, vui lòng liên hệ với chúng tôi ngay lập tức.</p>";

            await _emailService.SendEmailAsync(user.Email, "Thông báo cập nhật tài khoản", emailBody);

            var userResponseDto = _mapper.Map<UserResponseDto>(user);
            return Ok(userResponseDto);
        }



    }


}

public class UserUpdateDto
{
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public string? Avatar { get; set; }
    public string? Password { get; set; }
    public string? OldPassword { get; set; }
}
public class ForgotPasswordDto
{
    public string? Email { get; set; }
}
public class LoginDto
{
    [EmailAddress]
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class RegisterDto
{
    public required string FullName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Mật khẩu phải có ít nhất một chữ cái viết hoa, một chữ cái viết thường, một chữ số và một ký tự đặc biệt.")]
    public required string Password { get; set; }
    public RoleStatus Role { get; set; } = RoleStatus.Customer;
    public UserStatus Status { get; set; } = UserStatus.Active;
}

public class OtpCookieDto
{
    public string? Email { get; set; }
    public string? Otp { get; set; }
    public int Attempts { get; set; }
}

public class OtpVerifyDto
{
    public string? Email { get; set; }
    public string? Otp { get; set; }
}

public class ResendOtpDto
{
    public string? Email { get; set; }
}


