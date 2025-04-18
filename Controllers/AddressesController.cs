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
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AddressesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetAddressesByUserId()
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
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }
            Console.WriteLine("alo:" + userId);
            var addresses = await _context.Addresses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            var addressDtos = _mapper.Map<List<AddressResponseDto>>(addresses);
            return Ok(new { addressDtos });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateAddress([FromBody] AddressCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
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

            if (dto.Active)
            {
                var addresses = await _context.Addresses
                    .Where(a => a.UserId == userId && a.Active)
                    .ToListAsync();

                foreach (var a in addresses)
                {
                    a.Active = false;
                }
            }

            var address = _mapper.Map<Address>(dto);
            address.UserId = userId;
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAddressesByUserId), new { userId = address.UserId }, _mapper.Map<AddressResponseDto>(address));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAddress(int id, [FromBody] AddressUpdateDto dto)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound("Địa chỉ không tồn tại.");

            // Cập nhật từng trường nếu có
            if (!string.IsNullOrEmpty(dto.FullName)) address.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.PhoneNumber)) address.PhoneNumber = dto.PhoneNumber;
            if (!string.IsNullOrEmpty(dto.AddressDetail)) address.AddressDetail = dto.AddressDetail;
            if (!string.IsNullOrEmpty(dto.Ward)) address.Ward = dto.Ward;
            if (!string.IsNullOrEmpty(dto.District)) address.District = dto.District;
            if (!string.IsNullOrEmpty(dto.City)) address.City = dto.City;

            // Nếu người dùng chọn địa chỉ này là active => các địa chỉ khác phải false
            if (dto.Active)
            {
                var otherAddresses = await _context.Addresses
                    .Where(a => a.UserId == address.UserId && a.Id != id && a.Active)
                    .ToListAsync();

                foreach (var a in otherAddresses)
                {
                    a.Active = false;
                }

                address.Active = true;
            }
            else
            {
                address.Active = false;
            }

            _context.Update(address);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<AddressResponseDto>(address));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound("Địa chỉ không tồn tại.");

            if (address.Active)
                return BadRequest("Không thể xóa địa chỉ đang được sử dụng (Active = true).");

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }

}