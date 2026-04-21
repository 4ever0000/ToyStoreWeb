using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Review;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions; // Exception-lar üçün lazımdır

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

        [HttpGet("product/{productId}")]
        [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateReview(ReviewCreateDto request)
        {
            // Validasiya yoxlaması nümunəsi:
            if (request.Rating < 1 || request.Rating > 5)
                throw new BadRequestException("Rating (qiymət) 1 ilə 5 arasında olmalıdır.");

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
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);

            if (review == null)
                throw new NotFoundException(nameof(Review), id);

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}