using Microsoft.AspNetCore.Mvc;
using ToyStore.Application.DTOs.Brand;
using ToyStore.Models;
using ToyStore.Repositories;

namespace ToyStore.Controllers
{
    [ApiController]
    [Route("api/v1/brands")]
    public class BrandsController : ControllerBase
    {
        private readonly IGenericRepository<Brand> _brandRepository;

        // Constructor injection - düzgün üsul
        public BrandsController(IGenericRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllBrands()
        {
            var brands = await _brandRepository.GetAllAsync();

            List<BrandDto> result = new List<BrandDto>();

            foreach (var brand in brands)
            {
                result.Add(new BrandDto
                {
                    Id = brand.Id,
                    Name = brand.Name
                });
            }

            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return NotFound(new { Message = "Brend tapılmadı" });
            }

            BrandDto dto = new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name
            };

            return Ok(dto);
        }


        [HttpPost]
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
        public async Task<IActionResult> UpdateBrand(int id, BrandUpdateDto request)
        {
            if (id != request.Id)
            {
                return BadRequest(new { Message = "Uyğunsuz ID" });
            }

            var existBrand = await _brandRepository.GetByIdAsync(id);

            if (existBrand == null)
            {
                return NotFound(new { Message = "Brend tapılmadı" });
            }

            // Yalnız dəyişən sahələri yaz
            existBrand.Name = request.Name;

            _brandRepository.Update(existBrand);
            await _brandRepository.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBrand(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);

            if (brand == null)
            {
                return NotFound(new { Message = "Brend tapılmadı" });
            }

            _brandRepository.Delete(brand);
            await _brandRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}