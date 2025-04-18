using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NguyenSao_2122110145.Data;
using NguyenSao_2122110145.DTOs;
using NguyenSao_2122110145.Models;
using System.Security.Claims;
using OrderStatus = NguyenSao_2122110145.Models.OrderStatus;

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
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Include(o => o.DiscountCode)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Color)
                .ToListAsync();

            var orderDtos = _mapper.Map<List<OrderResponseDto>>(orders);
            return Ok(orderDtos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Customer,Staff,Manager,Admin")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Address)
                .Include(o => o.PaymentMethod)
                .Include(o => o.DiscountCode)
                .Include(o => o.OrderDetails).ThenInclude(od => od.Color)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (User.IsInRole("Customer"))
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                    return Forbid("Không tìm thấy thông tin người dùng.");

                var userId = int.Parse(claim.Value);
                if (order.UserId != userId)
                    return Forbid();
            }

            var orderDto = _mapper.Map<OrderResponseDto>(order);
            return Ok(orderDto);
        }


        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto orderDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return Forbid("Không tìm thấy thông tin người dùng.");
            }

            var userId = int.Parse(claim.Value);

            if (orderDto.UserId != userId)
                return Forbid("Bạn chỉ có thể tạo đơn hàng cho chính mình.");

            if (orderDto.UserId != userId)
                return Forbid("Bạn chỉ có thể tạo đơn hàng cho chính mình.");

            foreach (var detail in orderDto.OrderDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ColorId == detail.ColorId);
                if (inventory == null || inventory.Quantity < detail.Quantity)
                    return BadRequest($"Sản phẩm {detail.ColorId} không đủ hàng.");
            }

            var order = _mapper.Map<Order>(orderDto);
            order.Status = OrderStatus.Pending;

            order.TotalAmount = order.OrderDetails.Sum(od => od.Quantity * od.Price);
            order.FinalAmount = order.TotalAmount;

            if (orderDto.DiscountCodeId.HasValue)
            {
                var discountCode = await _context.DiscountCodes
                    .FirstOrDefaultAsync(dc => dc.Id == orderDto.DiscountCodeId && dc.IsActive && dc.UsedCount < dc.UsageLimit && dc.EndDate >= DateTime.UtcNow);
                if (discountCode == null)
                    return BadRequest("Mã giảm giá không hợp lệ hoặc đã hết hạn.");

                if (discountCode.DiscountPercent.HasValue)
                    order.FinalAmount = order.TotalAmount * (1 - discountCode.DiscountPercent.Value / 100);
                else if (discountCode.DiscountAmount.HasValue)
                    order.FinalAmount = order.TotalAmount - discountCode.DiscountAmount.Value;

                if (order.FinalAmount < 0)
                    order.FinalAmount = 0;

                discountCode.UsedCount++;
            }

            foreach (var detail in order.OrderDetails)
            {
                var inventory = await _context.Inventories
                    .FirstOrDefaultAsync(i => i.ColorId == detail.ColorId);

                if (inventory == null)
                {
                    return BadRequest($"Sản phẩm màu {detail.ColorId} không có trong kho.");
                }

                inventory.Quantity -= detail.Quantity;
            }


            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<OrderResponseDto>(order);
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, responseDto);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Staff,Manager,Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound();

            order.Status = status;

            await _context.SaveChangesAsync();

            var responseDto = _mapper.Map<OrderResponseDto>(order);
            return Ok(responseDto);
        }
    }
}