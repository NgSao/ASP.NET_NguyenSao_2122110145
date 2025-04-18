
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NguyenSao_2122110145.Data;
using System.Security.Claims;
using System.Text.RegularExpressions;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using NguyenSao_2122110145.Models;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MediaController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<MediaController> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _githubToken;
    private readonly string _repoOwner;
    private readonly string _repoName;
    private readonly string _branch;

    public MediaController(
        AppDbContext context,
        IMapper mapper,
        ILogger<MediaController> logger,
        IHttpClientFactory httpClientFactory,
            IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _githubToken = configuration["GitHub:PersonalAccessToken"]!;
        _repoOwner = configuration["GitHub:RepoOwner"]!;
        _repoName = configuration["GitHub:RepoName"]!;
        _branch = configuration["GitHub:Branch"]!;
    }


    [HttpPost("upload/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadAvatarRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        // Xác thực định dạng và kích thước
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
            // Chuyển file sang base64
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var base64Content = Convert.ToBase64String(memoryStream.ToArray());

            // Tạo tên file an toàn
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var sanitizedFileName = Regex.Replace(request.File.FileName, "[^a-zA-Z0-9.-]", "_");
            var path = $"asp/users/{timestamp}_{sanitizedFileName}";

            // Tải lên GitHub
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
                _logger.LogError("GitHub upload failed: {Error}", errorContent);
                return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
            }

            var imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/main/{path}";
            _logger.LogInformation("Image uploaded to GitHub: {ImageUrl}", imageUrl);

            var userClaim = User.Claims
                        .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _))
                        .FirstOrDefault();

            if (userClaim == null)
            {
                return Unauthorized(new { message = "Không tìm thấy claim định danh người dùng hợp lệ." });
            }


            if (!int.TryParse(userClaim.Value, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            user.Avatar = imageUrl;
            user.FullName = request.fullName;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Image uploaded and avatar updated successfully.", imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while uploading the image.", error = ex.Message });
        }
    }



    [HttpPost("upload/brand")]
    [Authorize(Roles = "Manager,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreateBrandImage([FromForm] UploadBrandRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        // Xác thực định dạng và kích thước
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
            // Chuyển file sang base64
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var base64Content = Convert.ToBase64String(memoryStream.ToArray());

            // Tạo tên file an toàn
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var sanitizedFileName = Regex.Replace(request.File.FileName, "[^a-zA-Z0-9.-]", "_");
            var path = $"asp/brands/{timestamp}_{sanitizedFileName}";

            // Tải lên GitHub
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
                _logger.LogError("GitHub upload failed: {Error}", errorContent);
                return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
            }

            var imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/main/{path}";
            _logger.LogInformation("Image uploaded to GitHub: {ImageUrl}", imageUrl);

            var brand = new Brand
            {
                Name = request.Name,
                Slug = request.Slug,
                ImageUrl = imageUrl,
                Status = Status.Active
            };

            _context.Brands.Add(brand);


            await _context.SaveChangesAsync();

            return Ok(new { message = "Image uploaded and avatar updated successfully.", imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while uploading the image.", error = ex.Message });
        }
    }


    [HttpPut("upload/brand/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateBrandImage(int id, [FromForm] UploadBrandUpdated request)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null)
            return NotFound(new { message = "Brand not found." });

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
                var path = $"asp/brands/{timestamp}_{sanitizedFileName}";

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
                    _logger.LogError("GitHub upload failed: {Error}", errorContent);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
                }

                imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/{_branch}/{path}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading brand image");
                return StatusCode(500, new { message = "Error uploading image.", error = ex.Message });
            }
        }
        brand.Name = request.Name!;
        brand.Slug = request.Slug!;
        if (imageUrl != null)
        {
            brand.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Brand updated successfully.",
            imageUrl = brand.ImageUrl,
            brand
        });
    }



    [HttpPost("upload/category")]
    [Authorize(Roles = "Manager,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatCategoryImage([FromForm] UploadCategoryRequest request)
    {
        if (request.File == null || request.File.Length == 0)
        {
            return BadRequest(new { message = "No file uploaded." });
        }

        // Validate file format and size
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
            // Validate ParentId if provided
            if (request.ParentId.HasValue)
            {
                var parentCategory = await _context.Categories.FindAsync(request.ParentId.Value);
                if (parentCategory == null)
                {
                    return BadRequest(new { message = $"Parent category with ID {request.ParentId.Value} does not exist." });
                }
            }

            // Convert file to base64
            using var memoryStream = new MemoryStream();
            await request.File.CopyToAsync(memoryStream);
            var base64Content = Convert.ToBase64String(memoryStream.ToArray());

            // Create safe file name
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var sanitizedFileName = Regex.Replace(request.File.FileName, "[^a-zA-Z0-9.-]", "_");
            var path = $"asp/categories/{timestamp}_{sanitizedFileName}";

            // Upload to GitHub
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
                _logger.LogError("GitHub upload failed: {Error}", errorContent);
                return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
            }

            var imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/main/{path}";
            _logger.LogInformation("Image uploaded to GitHub: {ImageUrl}", imageUrl);

            var category = new Category
            {
                Name = request.Name,
                Slug = request.Slug,
                ImageUrl = imageUrl,
                ParentId = request.ParentId, // Nullable, validated above
                Status = Status.Active
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Image uploaded and category created successfully.", imageUrl });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image for user {UserId}", User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            return StatusCode(500, new { message = "An error occurred while uploading the image.", error = ex.Message });
        }
    }
    [HttpPut("upload/category/{id}")]
    [Authorize(Roles = "Manager,Admin")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateUpdatedImage(int id, [FromForm] UploadCategoryUpdated request)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null)
            return NotFound(new { message = "Category not found." });

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
                    _logger.LogError("GitHub upload failed: {Error}", errorContent);
                    return StatusCode((int)response.StatusCode, new { message = "Failed to upload image to GitHub.", error = errorContent });
                }

                imageUrl = $"https://raw.githubusercontent.com/{_repoOwner}/{_repoName}/{_branch}/{path}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading brand image");
                return StatusCode(500, new { message = "Error uploading image.", error = ex.Message });
            }
        }
        category.Name = request.Name!;
        category.Slug = request.Slug!;
        category.ParentId = request.ParentId!;
        if (imageUrl != null)
        {
            category.ImageUrl = imageUrl;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Category updated successfully.",
            imageUrl = category.ImageUrl,
            category
        });
    }


}


public class UploadAvatarRequest
{
    public required IFormFile File { get; set; }
    public required string fullName { get; set; }
}


public class UploadBrandRequest
{
    public required IFormFile File { get; set; }
    public required string Name { get; set; }

    public required string Slug { get; set; }

}

public class UploadBrandUpdated
{
    public IFormFile? File { get; set; }
    public string? Name { get; set; }

    public string? Slug { get; set; }

}


public class UploadCategoryRequest
{
    public required IFormFile File { get; set; }
    public required string Name { get; set; }

    public required string Slug { get; set; }

    public int? ParentId { get; set; }

}
public class UploadCategoryUpdated
{
    public IFormFile? File { get; set; }
    public string? Name { get; set; }

    public string? Slug { get; set; }

    public int? ParentId { get; set; }

}




