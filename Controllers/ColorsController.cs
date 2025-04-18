using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ColorsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ColorResponseDto>>> GetAll()
        {
            var colors = await _context.Colors
                .Include(c => c.Media)
                .ToListAsync();

            return Ok(_mapper.Map<IEnumerable<ColorResponseDto>>(colors));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ColorResponseDto>> GetById(int id)
        {
            var color = await _context.Colors
                .Include(c => c.Media)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
                return NotFound();

            return Ok(_mapper.Map<ColorResponseDto>(color));
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<ColorResponseDto>> Create(ColorCreateDto dto)
        {
            var color = _mapper.Map<Color>(dto);
            _context.Colors.Add(color);
            await _context.SaveChangesAsync();
            var inventory = new Inventory
            {
                ColorId = color.Id,
                Quantity = color.Stock
            };
            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = color.Id }, _mapper.Map<ColorResponseDto>(color));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> Update(int id, ColorUpdateDto dto)
        {
            var color = await _context.Colors
                .Include(c => c.Media)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (color == null)
                return NotFound();

            _mapper.Map(dto, color);

            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ColorId == id);
            if (inventory != null)
            {
                inventory.Quantity = color.Stock;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var color = await _context.Colors.FindAsync(id);
            if (color == null)
                return NotFound();

            _context.Colors.Remove(color);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}