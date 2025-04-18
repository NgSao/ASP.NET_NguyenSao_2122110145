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
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CategoriesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.Parent)
                .ToListAsync();
            var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Parent)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
                return NotFound();

            var categoryDto = _mapper.Map<CategoryResponseDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateDto categoryDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var category = _mapper.Map<Category>(categoryDto);
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<CategoryResponseDto>(category);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryCreateDto categoryDto)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _mapper.Map(categoryDto, category);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<CategoryResponseDto>(category);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound();

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}