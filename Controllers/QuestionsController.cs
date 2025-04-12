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
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public QuestionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestions()
        {
            var questions = await _context.Questions
                .Include(q => q.Product)
                .Include(q => q.User)
                .ToListAsync();

            var questionDtos = _mapper.Map<List<QuestionResponseDto>>(questions);
            return Ok(questionDtos);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetQuestion(int id)
        {
            var question = await _context.Questions
                .Include(q => q.Product)
                .Include(q => q.User)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question == null)
                return NotFound();

            var questionDto = _mapper.Map<QuestionResponseDto>(question);
            return Ok(questionDto);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateQuestion([FromBody] QuestionCreateDto questionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);



            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var question = _mapper.Map<Question>(questionDto);
            question.UserId = userId;
            question.CreatedAt = DateTime.UtcNow;

            _context.Questions.Add(question);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<QuestionResponseDto>(question);
            return CreatedAtAction(nameof(GetQuestion), new { id = question.Id }, responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null)
                return NotFound();

            if (User.IsInRole("Customer"))
            {


                var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaim == null)
                    return Unauthorized("Người dùng chưa đăng nhập.");

                var userId = int.Parse(userClaim.Value);
                if (question.UserId != userId)
                    return Forbid("Bạn chỉ có thể xóa câu hỏi của mình.");
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}