using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Review;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/reviews")]
    public class ReviewsController : ControllerBase
    {
        private readonly IGenericRepository<Review> _reviewRepo;

        public ReviewsController(IGenericRepository<Review> reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }


        // Məhsula aid bütün rəyləri al
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProductId(int productId)
        {
            var allReviews = await _reviewRepo.GetAllAsync();
            var reviews = allReviews.Where(r => r.ProductId == productId);

            var result = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                ProductId = r.ProductId,
                UserId = r.UserId,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList();

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> CreateReview(ReviewCreateDto request)
        {
            var newReview = new Review
            {
                ProductId = request.ProductId,
                UserId = request.UserId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _reviewRepo.AddAsync(newReview);
            await _reviewRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetByProductId),
                new { productId = newReview.ProductId },
                newReview);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null)
                return NotFound(new { Message = "Rəy tapılmadı" });

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}