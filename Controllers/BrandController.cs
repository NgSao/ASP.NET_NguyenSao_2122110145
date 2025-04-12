using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BrandController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/brands
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrands()
        {
            var brands = await _context.Brands
                .Include(b => b.Products)
                .ToListAsync();
            return Ok(brands);
        }

        // GET: api/brands/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBrand(int id)
        {
            var brand = await _context.Brands
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (brand == null)
            {
                return NotFound();
            }

            return Ok(brand);
        }

        // POST: api/brands
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateBrand([FromBody] Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Brands.Add(brand);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrand), new { id = brand.Id }, brand);
        }

        // PUT: api/brands/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateBrand(int id, [FromBody] Brand brand)
        {
            if (id != brand.Id)
            {
                return BadRequest();
            }

            var existingBrand = await _context.Brands.FindAsync(id);
            if (existingBrand == null)
            {
                return NotFound();
            }

            existingBrand.Name = brand.Name;
            existingBrand.ImageUrl = brand.ImageUrl;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/brands/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _context.Brands.FindAsync(id);
            if (brand == null)
            {
                return NotFound();
            }

            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}