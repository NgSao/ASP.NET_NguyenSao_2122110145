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
    public class VariantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public VariantsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<VariantResponseDto>>> GetAll()
        {
            var variants = await _context.Variants
                .Include(v => v.Colors)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<VariantResponseDto>>(variants));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<VariantResponseDto>> GetById(int id)
        {
            var variant = await _context.Variants
                .Include(v => v.Colors)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (variant == null)
                return NotFound();

            return Ok(_mapper.Map<VariantResponseDto>(variant));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<VariantResponseDto>> Create([FromBody] VariantCreateDto dto)
        {
            var variant = _mapper.Map<Variant>(dto);

            _context.Variants.Add(variant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = variant.Id }, _mapper.Map<VariantResponseDto>(variant));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] VariantUpdateDto dto)
        {
            var variant = await _context.Variants.FindAsync(id);
            if (variant == null)
                return NotFound();

            _mapper.Map(dto, variant);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _context.Variants.FindAsync(id);
            if (variant == null)
                return NotFound();

            _context.Variants.Remove(variant);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}