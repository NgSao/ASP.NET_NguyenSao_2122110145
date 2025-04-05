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
using Microsoft.AspNetCore.Mvc;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;
using System.Linq;

//Dùng swager

namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var products = _context.Products.ToList();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Product product)
        {
            if (string.IsNullOrEmpty(product.ImageUrl))
            {
                product.ImageUrl = "/images/default.png"; 
            }

            _context.Products.Add(product);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Product product)
        {
            var existingProduct = _context.Products.FirstOrDefault(p => p.Id == id);
            if (existingProduct == null) return NotFound();

            existingProduct.Name = product.Name;
            existingProduct.Price = product.Price;

            if (!string.IsNullOrEmpty(product.ImageUrl))
            {
                existingProduct.ImageUrl = product.ImageUrl;
            }

            _context.SaveChanges();
            return Ok(existingProduct);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();

            _context.Products.Remove(product);
            _context.SaveChanges();
            return Ok(new { message = "Deleted successfully" });
        }
    }
}
