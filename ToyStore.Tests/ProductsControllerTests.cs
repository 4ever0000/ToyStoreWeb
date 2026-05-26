using Moq;
using ToyStore.Controllers;
using ToyStore.Repositories;
using ToyStore.Models;
using ToyStore.Application.DTOs.Product;
using ToyStore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ToyStore.Tests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IGenericRepository<Product>> _mockRepo;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            // 1. Repository-nin saxtasını yaradırıq
            _mockRepo = new Mock<IGenericRepository<Product>>();

            // 2. Controller-ə həmin saxta obyekti veririk
            _controller = new ProductsController(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAllProducts_OnlyReturnsActiveProducts()
        {
            // Arrange (Hazırlıq)
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Aktiv Oyuncaq", Is_active = true },
                new Product { Id = 2, Name = "Passiv Oyuncaq", Is_active = false }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(products);

            // Act (İcra etmə)
            var result = await _controller.GetAllProducts();

            // Assert (Yoxlama)
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnProducts = Assert.IsType<List<ProductDto>>(okResult.Value);

            Assert.Single(returnProducts); // Yalnız 1 məhsul qayıtmalıdır (Is_active = true olan)
            Assert.Equal("Aktiv Oyuncaq", returnProducts[0].Name);
        }

        [Fact]
        public async Task GetProductById_ProductNotFound_ThrowsNotFoundException()
        {
            // Arrange
            _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((Product)null);

            // Act & Assert (Burada həm icra edirik, həm də xətanın atıldığını yoxlayırıq)
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetProductById(99));
        }

        [Fact]
        public async Task GetProductById_ProductIsInactive_ThrowsNotFoundException()
        {
            // Arrange
            var inactiveProduct = new Product { Id = 1, Is_active = false };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(inactiveProduct);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _controller.GetProductById(1));
        }

        [Fact]
        public async Task CreateProduct_ReturnsCreatedAtAction()
        {
            // Arrange
            var dto = new ProductCreateDto { Name = "Yeni Oyuncaq", Price = 50 };
            // AddAsync funksiyasının çağırılacağını simulyasiya edirik

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetProductById), createdResult.ActionName);

            // Verilənlər bazasına yazma əmri verilibmi? Yoxlayırıq:
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProduct_SetsIsActiveToFalse()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Silinəcək", Is_active = true };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(product);

            // Act
            var result = await _controller.DeleteProduct(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.False(product.Is_active); // Məhsulun Is_active dəyəri false olmalıdı (Soft delete)
            _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}