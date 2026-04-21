using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Category;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions; // Exception-lar üçün əlavə edildi

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    public class CategoriesController : ControllerBase
    {
        private readonly IGenericRepository<Category> _categoryRepo;

        public CategoriesController(IGenericRepository<Category> categoryRepo)
        {
            _categoryRepo = categoryRepo;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryRepo.GetAllAsync();

            var result = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                ParentId = c.Parent_id
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                throw new NotFoundException(nameof(Category), id);

            return Ok(new CategoryDto { Id = category.Id, Name = category.Name, ParentId = category.Parent_id });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(CategoryCreateDto request)
        {
            var category = new Category
            {
                Name = request.Name,
                Parent_id = request.ParentId
            };

            await _categoryRepo.AddAsync(category);
            await _categoryRepo.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(CategoryUpdateDto request)
        {
            var category = await _categoryRepo.GetByIdAsync(request.Id);

            if (category == null)
                throw new NotFoundException(nameof(Category), request.Id);

            category.Name = request.Name;
            category.Parent_id = request.ParentId;

            await _categoryRepo.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);

            if (category == null)
                throw new NotFoundException(nameof(Category), id);

            _categoryRepo.Delete(category);
            await _categoryRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}