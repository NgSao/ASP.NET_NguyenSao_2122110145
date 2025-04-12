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
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .ToListAsync();

            var reviewDtos = _mapper.Map<List<ReviewResponseDto>>(reviews);
            return Ok(reviewDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReview(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (review == null)
                return NotFound();

            var reviewDto = _mapper.Map<ReviewResponseDto>(review);
            return Ok(reviewDto);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto reviewDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var existingReview = await _context.Reviews
               .AnyAsync(r => r.ProductId == reviewDto.ProductId && r.UserId == userId);
            if (existingReview)
                return BadRequest("Bạn đã đánh giá sản phẩm này rồi.");

            var review = _mapper.Map<Review>(reviewDto);
            review.UserId = userId;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ReviewResponseDto>(review);
            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, responseDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] ReviewCreateDto reviewDto)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            if (review.UserId != userId)
                return Forbid("Bạn chỉ có thể chỉnh sửa đánh giá của mình.");

            _mapper.Map(reviewDto, review);

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<ReviewResponseDto>(review);
            return Ok(responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
                return NotFound();

            if (User.IsInRole("Customer"))
            {

                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaim == null)
                    return Unauthorized("Người dùng chưa đăng nhập.");

                var userId = int.Parse(userClaim.Value);
                if (review.UserId != userId)
                    return Forbid("Bạn chỉ có thể xóa đánh giá của mình.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}