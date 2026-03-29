using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Product;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IGenericRepository<Product> _productRepo;

        public ProductsController(IGenericRepository<Product> productRepo)
        {
            _productRepo = productRepo;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productRepo.GetAllAsync();
            var activeProducts = products.Where(p => p.Is_active).ToList();

            var result = activeProducts.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                StockQuantity = p.Stock_quantity,
                CategoryId = p.Category_id,
                BrandId = p.Brand_id,
                ImageUrl = p.Image_url,
                IsActive = p.Is_active
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null || !product.Is_active) return NotFound(new { Message = "Məhsul tapılmadı" });

            return Ok(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                StockQuantity = product.Stock_quantity,
                CategoryId = product.Category_id,
                BrandId = product.Brand_id,
                ImageUrl = product.Image_url,
                IsActive = product.Is_active
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateDto request)
        {
            var product = new Product
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                Stock_quantity = request.StockQuantity,
                Category_id = request.CategoryId,
                Brand_id = request.BrandId,
                Image_url = request.ImageUrl,
                Is_active = request.IsActive,
                Created_at = DateTime.UtcNow
            };

            await _productRepo.AddAsync(product);
            await _productRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProductById), new { id = product.Id }, product);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(ProductUpdateDto request)
        {
            var product = await _productRepo.GetByIdAsync(request.Id);
            if (product == null) return NotFound();

            product.Name = request.Name;
            product.Price = request.Price;
            product.Description = request.Description;
            product.Stock_quantity = request.StockQuantity;
            product.Category_id = request.CategoryId;
            product.Brand_id = request.BrandId;
            product.Image_url = request.ImageUrl;
            product.Is_active = request.IsActive;

            await _productRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productRepo.GetByIdAsync(id);
            if (product == null) return NotFound();

            product.Is_active = false;
            await _productRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}