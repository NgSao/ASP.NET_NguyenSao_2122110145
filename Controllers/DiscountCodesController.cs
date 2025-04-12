using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class DiscountCodesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public DiscountCodesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Customer,Manager,Admin")]
        public async Task<IActionResult> GetDiscountCodes()
        {
            var discountCodes = await _context.DiscountCodes
                .Where(dc => dc.IsActive && dc.UsedCount < dc.UsageLimit && dc.EndDate >= DateTime.UtcNow)
                .ToListAsync();

            var discountCodeDtos = _mapper.Map<List<DiscountCodeResponseDto>>(discountCodes);
            return Ok(discountCodeDtos);
        }

        [HttpGet("{code}")]
        [Authorize(Roles = "Customer,Manager,Admin")]
        public async Task<IActionResult> GetDiscountCodeByCode(string code)
        {
            var discountCode = await _context.DiscountCodes
                .FirstOrDefaultAsync(dc => dc.Code == code && dc.IsActive && dc.UsedCount < dc.UsageLimit && dc.EndDate >= DateTime.UtcNow);

            if (discountCode == null)
                return NotFound("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

            var discountCodeDto = _mapper.Map<DiscountCodeResponseDto>(discountCode);
            return Ok(discountCodeDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateDiscountCode([FromBody] DiscountCodeCreateDto discountCodeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (discountCodeDto.DiscountAmount == null && discountCodeDto.DiscountPercent == null)
                return BadRequest("Phải cung cấp DiscountAmount hoặc DiscountPercent.");

            var existingCode = await _context.DiscountCodes
                .AnyAsync(dc => dc.Code == discountCodeDto.Code);
            if (existingCode)
                return BadRequest("Mã giảm giá đã tồn tại.");

            var discountCode = _mapper.Map<DiscountCode>(discountCodeDto);
            discountCode.IsActive = true;
            _context.DiscountCodes.Add(discountCode);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<DiscountCodeResponseDto>(discountCode);
            return CreatedAtAction(nameof(GetDiscountCodeByCode), new { code = discountCode.Code }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateDiscountCode(int id, [FromBody] DiscountCodeCreateDto discountCodeDto)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
                return NotFound();

            _mapper.Map(discountCodeDto, discountCode);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<DiscountCodeResponseDto>(discountCode);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDiscountCode(int id)
        {
            var discountCode = await _context.DiscountCodes.FindAsync(id);
            if (discountCode == null)
                return NotFound();

            _context.DiscountCodes.Remove(discountCode);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}