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
    public class ImagesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ImagesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetImages()
        {
            var images = await _context.Images
                .Include(i => i.Product)
                .ToListAsync();

            var imageDtos = _mapper.Map<List<ImageResponseDto>>(images);
            return Ok(imageDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetImage(int id)
        {
            var image = await _context.Images
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (image == null)
                return NotFound();

            var imageDto = _mapper.Map<ImageResponseDto>(image);
            return Ok(imageDto);
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        public async Task<IActionResult> CreateImage([FromBody] ImageCreateDto imageDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var image = _mapper.Map<Image>(imageDto);
            _context.Images.Add(image);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ImageResponseDto>(image);
            return CreatedAtAction(nameof(GetImage), new { id = image.Id }, responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
                return NotFound();

            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}