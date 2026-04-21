using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Order;
using ToyStore.Application.DTOs.OrderItem;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions; // Özəl exception-lar üçün

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

        [HttpGet("user/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
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

        [HttpGet("{id}/items")]
        [ProducesResponseType(typeof(IEnumerable<OrderItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetOrderItems(int id)
        {
            // Sifarişin özünün varlığını yoxlamaq yaxşı olar
            var order = await _orderRepo.GetByIdAsync(id);
            if (order == null) throw new NotFoundException(nameof(Order), id);

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

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateOrder(OrderCreateDto request)
        {
            var allCart = await _cartRepo.GetAllAsync();
            var userCartItems = allCart.Where(c => c.UserId == request.UserId).ToList();

            if (!userCartItems.Any())
                throw new BadRequestException("Səbət boşdur, sifariş yaradıla bilməz.");

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

            decimal totalSum = 0;
            var allProducts = await _productRepo.GetAllAsync();

            foreach (var cartItem in userCartItems)
            {
                var product = allProducts.FirstOrDefault(p => p.Id == cartItem.ProductId);

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

            newOrder.TotalAmount = totalSum;
            await _orderRepo.SaveChangesAsync();

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

        [HttpPut("{id}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeOrderStatus(int id, [FromBody] string newStatus)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException(nameof(Order), id);

            order.Status = newStatus;
            await _orderRepo.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var order = await _orderRepo.GetByIdAsync(id);

            if (order == null)
                throw new NotFoundException(nameof(Order), id);

            order.Status = "Müştəri tərəfindən ləğv edildi";
            await _orderRepo.SaveChangesAsync();

            return Ok(new { Message = "Sifariş ləğv edildi" });
        }
    }
}