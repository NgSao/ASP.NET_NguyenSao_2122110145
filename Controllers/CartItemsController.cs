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
    public class CartItemsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public CartItemsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCartItems()
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.ProductColor).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .ToListAsync();

            var cartItemDtos = _mapper.Map<List<CartItemResponseDto>>(cartItems);
            return Ok(cartItemDtos);
        }

        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> AddToCart([FromBody] CartItemCreateDto cartItemDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var inventory = await _context.Inventories
                .FirstOrDefaultAsync(i => i.ProductColorId == cartItemDto.ProductColorId);

            if (inventory == null || inventory.Quantity < cartItemDto.Quantity)
                return BadRequest("Không đủ hàng trong kho.");

            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.UserId == userId && ci.ProductColorId == cartItemDto.ProductColorId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += cartItemDto.Quantity;
                if (inventory.Quantity < existingCartItem.Quantity)
                    return BadRequest("Không đủ hàng trong kho.");
            }
            else
            {
                var cartItem = _mapper.Map<CartItem>(cartItemDto);
                cartItem.UserId = userId;
                _context.CartItems.Add(cartItem);
            }

            await _context.SaveChangesAsync();

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.ProductColor).ThenInclude(pc => pc.Variant).ThenInclude(v => v.Product)
                .ToListAsync();
            var responseDtos = _mapper.Map<List<CartItemResponseDto>>(cartItems);
            return Ok(responseDtos);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var userClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userClaim == null)
                return Unauthorized("Người dùng chưa đăng nhập.");

            var userId = int.Parse(userClaim.Value);
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(ci => ci.Id == id && ci.UserId == userId);

            if (cartItem == null)
                return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}