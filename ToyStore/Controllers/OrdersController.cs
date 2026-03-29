using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Order;
using ToyStore.Application.DTOs.OrderItem;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<OrderItem> _orderItemRepo;
        private readonly IGenericRepository<Cart> _cartRepo;
        private readonly IGenericRepository<Product> _productRepo;

        public OrdersController(
            IGenericRepository<Order> orderRepo,
            IGenericRepository<OrderItem> orderItemRepo,
            IGenericRepository<Cart> cartRepo,
            IGenericRepository<Product> productRepo)
        {
            _orderRepo = orderRepo;
            _orderItemRepo = orderItemRepo;
            _cartRepo = cartRepo;
            _productRepo = productRepo;
        }


        // ✅ İstifadəçinin bütün sifarişləri
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserOrders(int userId)
        {
            var allOrders = await _orderRepo.GetAllAsync();
            var userOrders = allOrders.Where(o => o.UserId == userId).ToList();

            var result = userOrders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                Status = o.Status,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                CreatedAt = o.CreatedAt
            }).ToList();

            return Ok(result);
        }


        // ✅ Sifariş daxilindəki məhsullar
        [HttpGet("{id}/items")]
        public async Task<IActionResult> GetOrderItems(int id)
        {
            var allItems = await _orderItemRepo.GetAllAsync();
            var orderItems = allItems.Where(i => i.OrderId == id).ToList();

            var result = orderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList();

            return Ok(result);
        }


        // ✅ ƏSAS METOD: Səbətdən tam sifariş yarat
        [HttpPost]
        public async Task<IActionResult> CreateOrder(OrderCreateDto request)
        {
            // 1. Səbəti al
            var allCart = await _cartRepo.GetAllAsync();
            var userCartItems = allCart.Where(c => c.UserId == request.UserId).ToList();

            if (!userCartItems.Any())
                return BadRequest(new { Message = "Səbət boşdur, sifariş yaradıla bilməz" });


            // 2. Yeni boş sifariş yarat
            var newOrder = new Order
            {
                UserId = request.UserId,
                Status = "Gözləmədə",
                ShippingAddress = request.ShippingAddress,
                TotalAmount = 0,
                CreatedAt = DateTime.UtcNow
            };

            await _orderRepo.AddAsync(newOrder);
            await _orderRepo.SaveChangesAsync();


            // 3. Bütün səbət məhsullarını sifarişə köçür, həqiqi qiyməti çək
            decimal totalSum = 0;
            var allProducts = await _productRepo.GetAllAsync();

            foreach (var cartItem in userCartItems)
            {
                var product = allProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

                // Əgər məhsul artıq silinibsə
                if (product == null || !product.Is_active)
                    continue;

                var orderItem = new OrderItem
                {
                    OrderId = newOrder.Id,
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price
                };

                totalSum += orderItem.UnitPrice * orderItem.Quantity;
                await _orderItemRepo.AddAsync(orderItem);
            }

            // Sifariş ümumi məbləğini yadda saxla
            newOrder.TotalAmount = totalSum;
            await _orderRepo.SaveChangesAsync();


            // 4. Səbəti tam təmizlə
            foreach (var cartItem in userCartItems)
            {
                _cartRepo.Delete(cartItem);
            }
            await _cartRepo.SaveChangesAsync();


            return Ok(new
            {
                OrderId = newOrder.Id,
                Total = totalSum,
                Message = "Sifariş uğurla qəbul olundu"
            });
        }


        // ✅ Sifariş statusunu dəyiş (admin üçün)
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                return NotFound(new { Message = "Belə sifariş mövcud deyil" });

            order.Status = newStatus;
            await _orderRepo.SaveChangesAsync();

            return NoContent();
        }


        // ✅ Sifarişi ləğv et
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                return NotFound();

            order.Status = "Müştəri tərəfindən ləğv edildi";
            await _orderRepo.SaveChangesAsync();

            return Ok(new { Message = "Sifariş ləğv edildi" });
        }
    }
}