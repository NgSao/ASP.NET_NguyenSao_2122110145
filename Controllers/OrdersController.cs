using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;

namespace NguyenSao_2122110145.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public OrdersController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            var result = _mapper.Map<List<OrderResponseDto>>(orders);
            return Ok(result);
        }

        [HttpPut("status/{id}")]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateDto request)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            if (!Enum.TryParse<OrderStatus>(request.Status, true, out var newStatus))
                return BadRequest("Trạng thái không hợp lệ.");

            int current = (int)order.Status;
            int next = (int)newStatus;

            if (newStatus == OrderStatus.Cancelled)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Đơn hàng đã được hủy." });
            }

            if (next != current + 1)
                return BadRequest("Chỉ được phép cập nhật trạng thái theo thứ tự tăng dần từng bước (trừ khi hủy).");

            order.Status = newStatus;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cập nhật trạng thái đơn hàng thành công." });
        }




        [HttpGet("admin/{id}")]
        [Authorize(Roles = "Admin,Manager,Staff")]
        public async Task<IActionResult> GetOrderByIdAdmin(int id)
        {
            var order = await _context.Orders
                .Where(o => o.Id == id)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .Include(o => o.User)
                .FirstOrDefaultAsync();

            if (order == null)
                return NotFound("Không tìm thấy đơn hàng.");

            var result = _mapper.Map<OrderResponseDto>(order);
            return Ok(result);
        }



        [HttpGet("user")]
        [Authorize]
        public async Task<IActionResult> GetMyOrders()
        {
            var userClaim = User.Claims
                         .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _))
                         .FirstOrDefault();

            if (userClaim == null)
            {

                return Unauthorized("Không tìm thấy claim định danh người dùng hợp lệ.");
            }


            if (!int.TryParse(userClaim.Value, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }

            var orders = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .ToListAsync();

            var result = _mapper.Map<List<OrderResponseDto>>(orders);
            return Ok(result);
        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var userClaim = User.Claims
                         .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _))
                         .FirstOrDefault();

            if (userClaim == null)
            {

                return Unauthorized("Không tìm thấy claim định danh người dùng hợp lệ.");
            }


            if (!int.TryParse(userClaim.Value, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            var result = _mapper.Map<OrderResponseDto>(order);
            return Ok(result);
        }





        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto request)
        {
            var userClaim = User.Claims
                         .Where(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" && int.TryParse(c.Value, out _))
                         .FirstOrDefault();

            if (userClaim == null)
            {

                return Unauthorized("Không tìm thấy claim định danh người dùng hợp lệ.");
            }


            if (!int.TryParse(userClaim.Value, out var userId))
            {
                return BadRequest(new { message = "ID người dùng không hợp lệ." });
            }
            var order = new Order
            {
                UserId = userId,
                AddressId = request.AddressId,
                PaymentMethod = request.PaymentMethod,
                Status = OrderStatus.Pending,
                TotalAmount = request.TotalAmount,
                FinalAmount = request.FinalAmount,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var item in request.OrderDetails)
            {
                var product = await _context.Products.FindAsync(item.ProductId);
                if (product == null)
                {
                    return BadRequest($"Sản phẩm với ID {item.ProductId} không tồn tại.");
                }

                if (product.Stock < item.Quantity)
                {
                    return BadRequest($"Sản phẩm {product.Name} không đủ hàng trong kho.");
                }

                product.Stock -= item.Quantity;
                product.Sold += item.Quantity;

                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };

                _context.OrderDetails.Add(orderDetail);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Đặt hàng thành công", orderId = order.Id });
        }

    }
}