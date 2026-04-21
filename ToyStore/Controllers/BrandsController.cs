using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Brand;
using ToyStore.Models;
using ToyStore.Repositories;
using ToyStore.Application.Common.Exceptions; // Exception-lar üçün lazımdır

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly IGenericRepository<Brand> _brandRepository;

        public BrandsController(IGenericRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<BrandDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandRepository.GetAllAsync();

            // Select istifadə edərək kodu daha da qısaltdım (foreach ilə eyni işi görür)
            var result = brands.Select(brand => new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            }).ToList();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BrandDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                // Artıq manual mesaj yazmağa ehtiyac yoxdur, Exception klası bunu edir
                throw new NotFoundException(nameof(Brand), id);
            }

            return Ok(new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            });
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBrand(BrandCreateDto request)
        {
            Brand newBrand = new Brand
            {
                Name = request.Name
            };

            await _brandRepository.AddAsync(newBrand);
            await _brandRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBrandById), new { id = newBrand.Id }, newBrand);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateBrand(int id, BrandUpdateDto request)
        {
            if (id != request.Id)
            {
                // BadRequestException istifadə edirik
                throw new BadRequestException("Gömndərilən ID ilə obyektdəki ID uyğun gəlmir.");
            }

            var existBrand = await _brandRepository.GetByIdAsync(id);

            if (existBrand == null)
            {
                throw new NotFoundException(nameof(Brand), id);
            }

            existBrand.Name = request.Name;

            _brandRepository.Update(existBrand);
            await _brandRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                throw new NotFoundException(nameof(Brand), id);
            }

            _brandRepository.Delete(brand);
            await _brandRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}