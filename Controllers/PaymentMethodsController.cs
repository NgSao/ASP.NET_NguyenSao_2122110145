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
    public class PaymentMethodsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public PaymentMethodsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaymentMethods()
        {
            var paymentMethods = await _context.PaymentMethods
                .Where(pm => pm.IsActive)
                .ToListAsync();

            var paymentMethodDtos = _mapper.Map<List<PaymentMethodResponseDto>>(paymentMethods);
            return Ok(paymentMethodDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
                return NotFound();

            var paymentMethodDto = _mapper.Map<PaymentMethodResponseDto>(paymentMethod);
            return Ok(paymentMethodDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreatePaymentMethod([FromBody] PaymentMethodCreateDto paymentMethodDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var paymentMethod = _mapper.Map<PaymentMethod>(paymentMethodDto);

            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<PaymentMethodResponseDto>(paymentMethod);
            return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentMethod.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, [FromBody] PaymentMethodCreateDto paymentMethodDto)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
                return NotFound();

            _mapper.Map(paymentMethodDto, paymentMethod);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<PaymentMethodResponseDto>(paymentMethod);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeletePaymentMethod(int id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null)
                return NotFound();

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}