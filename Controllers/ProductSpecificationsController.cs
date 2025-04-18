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
    public class ProductSpecificationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductSpecificationsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSpecifications()
        {
            var specifications = await _context.ProductSpecifications
                .Include(ps => ps.Product)
                .ToListAsync();

            var specificationDtos = _mapper.Map<List<ProductSpecificationResponseDto>>(specifications);
            return Ok(specificationDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductSpecification(int id)
        {
            var specification = await _context.ProductSpecifications
                .Include(ps => ps.Product)
                .FirstOrDefaultAsync(ps => ps.Id == id);

            if (specification == null)
                return NotFound();

            var specificationDto = _mapper.Map<ProductSpecificationResponseDto>(specification);
            return Ok(specificationDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateProductSpecification([FromBody] ProductSpecificationCreateDto specificationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var specification = _mapper.Map<ProductSpecification>(specificationDto);
            _context.ProductSpecifications.Add(specification);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductSpecificationResponseDto>(specification);
            return CreatedAtAction(nameof(GetProductSpecification), new { id = specification.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateProductSpecification(int id, [FromBody] ProductSpecificationUpdateDto specificationDto)
        {
            var specification = await _context.ProductSpecifications.FindAsync(id);
            if (specification == null)
                return NotFound();

            _mapper.Map(specificationDto, specification);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ProductSpecificationResponseDto>(specification);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductSpecification(int id)
        {
            var specification = await _context.ProductSpecifications.FindAsync(id);
            if (specification == null)
                return NotFound();

            _context.ProductSpecifications.Remove(specification);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}