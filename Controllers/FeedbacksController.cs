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
    public class FeedbacksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public FeedbacksController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedbacks()
        {
            var feedbacks = await _context.Feedbacks
                .Include(f => f.Question)
                .Include(f => f.User)
                .ToListAsync();

            var feedbackDtos = _mapper.Map<List<FeedbackResponseDto>>(feedbacks);
            return Ok(feedbackDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFeedback(int id)
        {
            var feedback = await _context.Feedbacks
                .Include(f => f.Question)
                .Include(f => f.User)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (feedback == null)
                return NotFound();

            var feedbackDto = _mapper.Map<FeedbackResponseDto>(feedback);
            return Ok(feedbackDto);
        }

        [HttpPost]
        [Authorize(Roles = "Customer,Staff,Manager,Admin")]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackCreateDto feedbackDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var feedback = _mapper.Map<Feedback>(feedbackDto);
            feedback.UserId = userId;
            _context.Feedbacks.Add(feedback);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<FeedbackResponseDto>(feedback);
            return CreatedAtAction(nameof(GetFeedback), new { id = feedback.Id }, responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            var feedback = await _context.Feedbacks.FindAsync(id);
            if (feedback == null)
                return NotFound();

            _context.Feedbacks.Remove(feedback);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}