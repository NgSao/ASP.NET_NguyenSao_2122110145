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
    public class InventoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public InventoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Warehouse,Manager,Admin")]
        public async Task<IActionResult> GetInventories()
        {
            var inventories = await _context.Inventories
                .Include(i => i.Color).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .ToListAsync();

            var inventoryDtos = _mapper.Map<List<InventoryResponseDto>>(inventories);
            return Ok(inventoryDtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Warehouse,Manager,Admin")]
        public async Task<IActionResult> GetInventory(int id)
        {
            var inventory = await _context.Inventories
                .Include(i => i.Color).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (inventory == null)
                return NotFound();

            var inventoryDto = _mapper.Map<InventoryResponseDto>(inventory);
            return Ok(inventoryDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Warehouse,Admin")]
        public async Task<IActionResult> UpdateInventory(int id, [FromBody] InventoryUpdateDto inventoryDto)
        {
            var inventory = await _context.Inventories.FindAsync(id);
            if (inventory == null)
                return NotFound();

            _mapper.Map(inventoryDto, inventory);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<InventoryResponseDto>(inventory);
            return Ok(responseDto);
        }
    }
}