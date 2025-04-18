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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ProductsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.Medias)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);
            return Ok(productDtos);
        }



        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Variants)
                .Include(p => p.Medias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            var productDto = _mapper.Map<ProductResponseDto>(product);
            return Ok(productDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<ActionResult<Product>> CreateProduct([FromBody] ProductCreateDto dto)
        {
            var product = _mapper.Map<Product>(dto);

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Thêm media nếu có
            if (dto.Medias != null && dto.Medias.Count > 0)
            {
                foreach (var img in dto.Medias)
                {
                    var media = _mapper.Map<Media>(img);  // Giả sử bạn cần map sang đối tượng Media
                    media.ProductId = product.Id;
                    _context.Medias.Add(media);
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _context.Products.Include(p => p.Medias).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();

            _mapper.Map(dto, product);

            // Cập nhật lại danh sách ảnh
            _context.Medias.RemoveRange(product.Medias ?? new List<Media>());
            if (dto.Images != null)
            {
                foreach (var img in dto.Images)
                {
                    var media = _mapper.Map<Media>(img);
                    media.ProductId = product.Id;
                    _context.Medias.Add(media);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Cập nhật thành công");
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Medias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            _context.Medias.RemoveRange(product.Medias ?? new List<Media>());
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok("Xóa thành công");
        }
    }
}