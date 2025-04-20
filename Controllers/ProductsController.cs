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
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _githubToken;
        private readonly string _repoOwner;
        private readonly string _repoName;
        private readonly string _branch;

        public ProductsController(AppDbContext context, IMapper mapper,
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
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);
            return Ok(new { data = productDtos });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound(new { message = "Product not found" });

            var productDto = _mapper.Map<ProductResponseDto>(product);
            return Ok(new { data = productDto });
        }



        [HttpGet("category/{categoryId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProductsByCategory(int categoryId)
        {
            var products = await _context.Products
                .Where(p => p.Category.Id == categoryId)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .ToListAsync();

            if (products == null || !products.Any())
                return NotFound(new { message = "Không tìm thấy sản phẩm nào trong danh mục này." });

            var productDtos = _mapper.Map<List<ProductResponseDto>>(products);
            return Ok(new { data = productDtos });
        }




        [HttpPost]
        [Authorize(Roles = "Manager,Admin")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateProduct([FromForm] ProductCreateDto request)
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
                var path = $"asp/products/{timestamp}_{sanitizedFileName}";

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

                var category = await _context.Categories.FindAsync(request.CategoryId);
                var brand = await _context.Brands.FindAsync(request.BrandId);

                if (category == null)
                {
                    return BadRequest(new { message = "Category not found." });
                }


                if (brand == null)
                {
                    return BadRequest(new { message = "Brand not found." });
                }

                var product = new Product
                {
                    Name = request.Name,
                    Slug = request.Slug,
                    Description = request.Description,
                    OriginalPrice = request.OriginalPrice,
                    SalePrice = request.SalePrice,
                    Stock = request.Stock,
                    Sold = request.Sold,
                    Category = category,
                    Brand = brand,
                    ImageUrl = imageUrl,
                };
                _context.Products.Add(product);
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
        public async Task<IActionResult> UpdateProduct(int id, [FromForm] ProductUpdateDto request)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Not found." });
            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Invalid CategoryId." });
            }

            var brandExists = await _context.Brands.AnyAsync(b => b.Id == request.BrandId);
            if (!brandExists)
            {
                return BadRequest(new { message = "Invalid BrandId." });
            }

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
                    var path = $"asp/products/{timestamp}_{sanitizedFileName}";

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


            product.Name = request.Name!;
            product.Slug = request.Slug!;
            product.Description = request.Description!;
            product.OriginalPrice = request.OriginalPrice;
            product.SalePrice = request.SalePrice;
            product.Stock = request.Stock;
            product.Sold = request.Sold;
            product.CategoryId = request.CategoryId;
            product.BrandId = request.BrandId;
            if (imageUrl != null)
            {
                product.ImageUrl = imageUrl;
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Product updated successfully.",
                imageUrl = product.ImageUrl,
                product
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return NotFound(new { message = "Product not found" });

            var orderDetails = await _context.OrderDetails
                .FirstOrDefaultAsync(od => od.ProductId == id);

            if (orderDetails != null)
            {
                return BadRequest(new { message = "Product cannot be deleted because it is part of an order" });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Product deleted successfully" });
        }

    }
}