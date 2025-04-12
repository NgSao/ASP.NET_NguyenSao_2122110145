using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.Models;
using System.Security.Claims;
namespace NguyenSao_2122110145.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrderDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/orderdetails
        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> GetOrderDetails()
        {
            var orderDetails = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .ToListAsync();
            return Ok(orderDetails);
        }

        // GET: api/orderdetails/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Staff,User")]
        public async Task<IActionResult> GetOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails
                .Include(od => od.Order)
                .Include(od => od.Product)
                .FirstOrDefaultAsync(od => od.Id == id);

            if (orderDetail == null)
            {
                return NotFound();
            }

            if (User.IsInRole("User"))
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
                if (orderDetail.Order?.UserId != userId)
                {
                    return Forbid();
                }
            }

            return Ok(orderDetail);
        }

        // POST: api/orderdetails
        [HttpPost]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> CreateOrderDetail([FromBody] OrderDetail orderDetail)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var order = await _context.Orders.FindAsync(orderDetail.OrderId);
            if (order == null)
            {
                return BadRequest("Đơn hàng không tồn tại.");
            }

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (User.IsInRole("User") && order.UserId != userId)
            {
                return Forbid();
            }

            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrderDetail), new { id = orderDetail.Id }, orderDetail);
        }

        // PUT: api/orderdetails/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] OrderDetail orderDetail)
        {
            if (id != orderDetail.Id)
            {
                return BadRequest();
            }

            var existingOrderDetail = await _context.OrderDetails.FindAsync(id);
            if (existingOrderDetail == null)
            {
                return NotFound();
            }

            existingOrderDetail.OrderId = orderDetail.OrderId;
            existingOrderDetail.ProductId = orderDetail.ProductId;
            existingOrderDetail.Quantity = orderDetail.Quantity;
            existingOrderDetail.UnitPrice = orderDetail.UnitPrice;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/orderdetails/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteOrderDetail(int id)
        {
            var orderDetail = await _context.OrderDetails.FindAsync(id);
            if (orderDetail == null)
            {
                return NotFound();
            }

            _context.OrderDetails.Remove(orderDetail);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}