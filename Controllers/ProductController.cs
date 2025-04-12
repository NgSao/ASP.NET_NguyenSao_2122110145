// using NguyenSao_2122110145.Data;
// using NguyenSao_2122110145.Models;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using System;
// using System.IO;
// using System.Linq;

// namespace NguyenSao_2122110145.Controllers
// {

//     public class ProductController : Controller
//     {
//         private readonly AppDbContext _context;

//         public ProductController(AppDbContext context)
//         {
//             _context = context;
//         }

//         public IActionResult Index()
//         {
//             var products = _context.Products.ToList();
//             return View(products);
//         }

//         public IActionResult Create()
//         {
//             return View();
//         }

//         [HttpPost]
//         public IActionResult Create(Product product, IFormFile? imageFile)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return View(product);
//             }

//             string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

//             if (!Directory.Exists(uploadsFolder))
//             {
//                 Directory.CreateDirectory(uploadsFolder);
//             }

//             if (imageFile != null && imageFile.Length > 0)
//             {
//                 string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
//                 string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };

//                 if (!allowedExtensions.Contains(fileExtension))
//                 {
//                     ModelState.AddModelError("ImageUrl", "Chỉ chấp nhận ảnh PNG, JPG, JPEG.");
//                     return View(product);
//                 }

//                 string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
//                 string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                 using (var stream = new FileStream(filePath, FileMode.Create))
//                 {
//                     imageFile.CopyTo(stream);
//                 }

//                 product.ImageUrl = "/images/" + uniqueFileName;
//             }
//             else
//             {
//                 product.ImageUrl = "/images/default.png";
//             }

//             _context.Products.Add(product);
//             _context.SaveChanges();
//             return RedirectToAction("Index");
//         }




//         //Cập nhật sản phẩm
//         public IActionResult Edit(int id)
//         {
//             var product = _context.Products.Find(id);
//             if (product == null)
//             {
//                 return NotFound();
//             }
//             return View(product);
//         }
//         [HttpPost]
//         public IActionResult Edit(Product product, IFormFile? imageFile)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return View(product);
//             }

//             var existingProduct = _context.Products.FirstOrDefault(p => p.Id == product.Id);
//             if (existingProduct == null)
//             {
//                 return NotFound();
//             }

//             existingProduct.Name = product.Name;
//             existingProduct.Price = product.Price;

//             if (imageFile != null && imageFile.Length > 0)
//             {
//                 string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

//                 if (!Directory.Exists(uploadsFolder))
//                 {
//                     Directory.CreateDirectory(uploadsFolder);
//                 }

//                 string fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
//                 string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };

//                 if (!allowedExtensions.Contains(fileExtension))
//                 {
//                     ModelState.AddModelError("ImageUrl", "Chỉ chấp nhận ảnh PNG, JPG, JPEG.");
//                     return View(product);
//                 }

//                 // Xóa ảnh cũ nếu không phải ảnh mặc định
//                 if (!string.IsNullOrEmpty(existingProduct.ImageUrl) && !existingProduct.ImageUrl.Contains("default.png"))
//                 {
//                     string oldImagePath = Path.Combine(uploadsFolder, Path.GetFileName(existingProduct.ImageUrl));
//                     if (System.IO.File.Exists(oldImagePath))
//                     {
//                         System.IO.File.Delete(oldImagePath);
//                     }
//                 }

//                 string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
//                 string filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                 using (var stream = new FileStream(filePath, FileMode.Create))
//                 {
//                     imageFile.CopyTo(stream);
//                 }

//                 existingProduct.ImageUrl = "/images/" + uniqueFileName;
//             }

//             _context.SaveChanges();
//             return RedirectToAction("Index");
//         }


//         // Xóa sản phẩm
//         public IActionResult Delete(int id)
//         {
//             var product = _context.Products.Find(id);
//             if (product == null) return NotFound();
//             return View(product);
//         }

//         [HttpPost, ActionName("Delete")]
//         public IActionResult DeleteConfirmed(int id)
//         {
//             var product = _context.Products.Find(id);
//             if (product != null)
//             {
//                 _context.Products.Remove(product);
//                 _context.SaveChanges();
//             }
//             return RedirectToAction("Index");
//         }
//     }
// }
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/products
        [HttpGet]
        [AllowAnonymous] // Mọi người đều có thể xem danh sách sản phẩm
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
            return Ok(products);
        }

        // GET: api/products/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        // POST: api/products
        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> CreateProduct([FromBody] Product product)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        // PUT: api/products/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            var existingProduct = await _context.Products.FindAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.ImageUrl = product.ImageUrl;
            existingProduct.Price = product.Price;
            existingProduct.CategoryId = product.CategoryId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/products/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Chỉ Admin được xóa
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}