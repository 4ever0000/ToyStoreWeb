using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Cart;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions; // Özəl exception-lar üçün

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

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<CartDto>), StatusCodes.Status200OK)]
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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddToCart(CartAddItemDto request)
        {
            // Validasiya: Miqdar 0 və ya mənfi ola bilməz
            if (request.Quantity <= 0)
                throw new BadRequestException("Məhsulun miqdarı 0-dan böyük olmalıdır.");

            var allItems = await _cartRepo.GetAllAsync();
            var exist = allItems.FirstOrDefault(c => c.UserId == request.UserId && c.ProductId == request.ProductId);

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

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateQuantity(CartUpdateQuantityDto request)
        {
            var item = await _cartRepo.GetByIdAsync(request.Id);

            if (item == null)
                throw new NotFoundException("Səbət elementi", request.Id);

            item.Quantity = request.Quantity;
            await _cartRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var item = await _cartRepo.GetByIdAsync(id);

            if (item == null)
                throw new NotFoundException("Səbət elementi", id);

            _cartRepo.Delete(item);
            await _cartRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}