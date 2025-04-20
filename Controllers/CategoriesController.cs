using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;

namespace NguyenSao_2122110145.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _githubToken;
        private readonly string _repoOwner;
        private readonly string _repoName;
        private readonly string _branch;

        public CategoriesController(AppDbContext context, IMapper mapper,
      IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _httpClientFactory = httpClientFactory;
            _githubToken = configuration["GitHub:PersonalAccessToken"]!;
            _repoOwner = configuration["GitHub:RepoOwner"]!;
            _repoName = configuration["GitHub:RepoName"]!;
            _branch = configuration["GitHub:Branch"]!;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories
                .Include(b => b.Products)
                .ToListAsync();

            var categoryDtos = _mapper.Map<List<CategoryResponseDto>>(categories);
            return Ok(new { data = categoryDtos });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (category == null)
                return NotFound(new { message = "Not Foud." });

            var categoryDto = _mapper.Map<CategoryResponseDto>(category);
            return Ok(new { data = categoryDto });
        }

        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateDto request)
        {



            if (request.File == null || request.File.Length == 0)
            {
                return BadRequest(new { message = "No file uploaded." });
            }

            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(request.File.FileName).ToLower();
            if (!validExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Only JPG, PNG, or GIF images are allowed." });
            }

            if (request.File.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "Image size must not exceed 5MB." });
            }

            try
            {
                using var memoryStream = new MemoryStream();
                await request.File.CopyToAsync(memoryStream);
                var base64Content = Convert.ToBase64String(memoryStream.ToArray());

                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var sanitizedFileName = Regex.Replace(request.File.FileName, "[^a-zA-Z0-9.-]", "_");
                var path = $"asp/categories/{timestamp}_{sanitizedFileName}";

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("NguyenSaoApp");

                var requestContent = new StringContent(
                    JsonSerializer.Serialize(new
                    {
                        message = $"Upload image: {sanitizedFileName}",
                        content = base64Content,
                        branch = _branch
                    }),
                    Encoding.UTF8, "application/json"
                );

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                var response = await client.PutAsync(
                    $"https://api.github.com/repos/{_repoOwner}/{_repoName}/contents/{path}",
                    requestContent
                );

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
                }

                var imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/main/{path}";

                var category = new Category
                {
                    Name = request.Name,
                    Slug = request.Slug,
                    ImageUrl = imageUrl,
                };
                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Image uploaded and avatar updated successfully.", imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while uploading the image.", error = ex.Message });
            }
        }



        [HttpPut("{id}")]
        [Authorize(Roles = "Manager,Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateCategory(int id, [FromForm] CategoryUpdateDto request)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                return NotFound(new { message = "Not found." });

            string? imageUrl = null;

            if (request.File != null && request.File.Length > 0)
            {
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(request.File.FileName).ToLower();

                if (!validExtensions.Contains(extension))
                    return BadRequest(new { message = "Only JPG, PNG, or GIF images are allowed." });

                if (request.File.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "Image size must not exceed 5MB." });

                try
                {
                    using var memoryStream = new MemoryStream();
                    await request.File.CopyToAsync(memoryStream);
                    var base64Content = Convert.ToBase64String(memoryStream.ToArray());

                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var sanitizedFileName = Regex.Replace(request.File.FileName, "[^a-zA-Z0-9.-]", "_");
                    var path = $"asp/categories/{timestamp}_{sanitizedFileName}";

                    var client = _httpClientFactory.CreateClient();
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("NguyenSaoApp");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _githubToken);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                    var uploadBody = new
                    {
                        message = $"Upload image: {sanitizedFileName}",
                        content = base64Content,
                        branch = _branch
                    };

                    var requestContent = new StringContent(
                        JsonSerializer.Serialize(uploadBody),
                        Encoding.UTF8, "application/json"
                    );

                    var response = await client.PutAsync(
                        $"https://api.github.com/repos/{_repoOwner}/{_repoName}/contents/{path}",
                        requestContent
                    );

                    if (!response.IsSuccessStatusCode)
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
                    }

                    imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/{_branch}/{path}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = "Error uploading image.", error = ex.Message });
                }
            }

            category.Name = request.Name!;
            category.Slug = request.Slug!;
            if (imageUrl != null)
            {
                category.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Brand updated successfully.",
                imageUrl = category.ImageUrl,
                category
            });
        }


        // Xóa danh mục
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(b => b.Products)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (category == null)
                return NotFound(new { message = "Not Foud." });

            if (category.Products != null && category.Products.Count > 0)
            {
                return BadRequest(new { message = "Không thể xóa thương hiệu vì đang có sản phẩm thuộc về nó." });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Oke" });
        }
    }
}
