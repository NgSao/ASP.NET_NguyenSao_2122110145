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
    public class ProductVariantsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductVariantsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductVariants()
        {
            var variants = await _context.ProductVariants
                .Include(pv => pv.Product)
                .ToListAsync();

            var variantDtos = _mapper.Map<List<ProductVariantResponseDto>>(variants);
            return Ok(variantDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductVariant(int id)
        {
            var variant = await _context.ProductVariants
                .Include(pv => pv.Product)
                .FirstOrDefaultAsync(pv => pv.Id == id);

            if (variant == null)
                return NotFound();

            var variantDto = _mapper.Map<ProductVariantResponseDto>(variant);
            return Ok(variantDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateProductVariant([FromBody] ProductVariantCreateDto variantDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var variant = _mapper.Map<ProductVariant>(variantDto);

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductVariantResponseDto>(variant);
            return CreatedAtAction(nameof(GetProductVariant), new { id = variant.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateProductVariant(int id, [FromBody] ProductVariantCreateDto variantDto)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
                return NotFound();

            _mapper.Map(variantDto, variant);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductVariantResponseDto>(variant);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductVariant(int id)
        {
            var variant = await _context.ProductVariants.FindAsync(id);
            if (variant == null)
                return NotFound();

            _context.ProductVariants.Remove(variant);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}