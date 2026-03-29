using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Cart;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/cart")]
    public class CartController : ControllerBase
    {
        private readonly IGenericRepository<Cart> _cartRepo;

        public CartController(IGenericRepository<Cart> cartRepo)
        {
            _cartRepo = cartRepo;
        }

        // İstifadəçinin səbətini al
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserCart(int userId)
        {
            var allItems = await _cartRepo.GetAllAsync();
            var userCart = allItems.Where(c => c.UserId == userId).ToList();

            var result = userCart.Select(c => new CartDto
            {
                Id = c.Id,
                UserId = c.UserId,
                ProductId = c.ProductId,
                Quantity = c.Quantity
            }).ToList();

            return Ok(result);
        }

        // Səbətə məhsul əlavə et
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartAddItemDto request)
        {
            var allItems = await _cartRepo.GetAllAsync();
            var exist = allItems.FirstOrDefault(c => c.UserId == request.UserId && c.ProductId == request.ProductId);

            // Əgər artıq səbətdədirsə sadəcə miqdarı artır
            if (exist != null)
            {
                exist.Quantity += request.Quantity;
                _cartRepo.Update(exist);
            }
            else
            {
                var newItem = new Cart
                {
                    UserId = request.UserId,
                    ProductId = request.ProductId,
                    Quantity = request.Quantity
                };
                await _cartRepo.AddAsync(newItem);
            }

            await _cartRepo.SaveChangesAsync();
            return Ok(new { Message = "Səbətə əlavə olundu" });
        }

        // Miqdarı dəyiş
        [HttpPut]
        public async Task<IActionResult> UpdateQuantity(CartUpdateQuantityDto request)
        {
            var item = await _cartRepo.GetByIdAsync(request.Id);
            if (item == null) return NotFound(new { Message = "Səbət elementi tapılmadı" });

            item.Quantity = request.Quantity;
            await _cartRepo.SaveChangesAsync();

            return NoContent();
        }

        // Səbətdən sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _cartRepo.GetByIdAsync(id);
            if (item == null) return NotFound();

            _cartRepo.Delete(item);
            await _cartRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}