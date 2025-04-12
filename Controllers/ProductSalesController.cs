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
    public class ProductSalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductSalesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSales()
        {
            var productSales = await _context.ProductSales
                .Include(ps => ps.ProductColor).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .ToListAsync();

            var productSaleDtos = _mapper.Map<List<ProductSaleResponseDto>>(productSales);
            return Ok(productSaleDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSale(int id)
        {
            var productSale = await _context.ProductSales
                .Include(ps => ps.ProductColor).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (productSale == null)
                return NotFound();

            var productSaleDto = _mapper.Map<ProductSaleResponseDto>(productSale);
            return Ok(productSaleDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateProductSale([FromBody] ProductSaleCreateDto productSaleDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (productSaleDto.DiscountAmount == null && productSaleDto.DiscountPercent == null)
                return BadRequest("Phải cung cấp DiscountAmount hoặc DiscountPercent.");

            var productSale = _mapper.Map<ProductSale>(productSaleDto);

            _context.ProductSales.Add(productSale);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductSaleResponseDto>(productSale);
            return CreatedAtAction(nameof(GetProductSale), new { id = productSale.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateProductSale(int id, [FromBody] ProductSaleCreateDto productSaleDto)
        {
            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale == null)
                return NotFound();

            _mapper.Map(productSaleDto, productSale);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductSaleResponseDto>(productSale);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductSale(int id)
        {
            var productSale = await _context.ProductSales.FindAsync(id);
            if (productSale == null)
                return NotFound();

            _context.ProductSales.Remove(productSale);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}