using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Product;

public class ProductCreateDto
{
    [Required, MinLength(3), MaxLength(200)]
    public string Name { get; set; }

    [Range(0.01, 100000)]
    public decimal Price { get; set; }

    public string Description { get; set; }

    [Range(0, 9999)]
    public int StockQuantity { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [Required]
    public int BrandId { get; set; }

    public string ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
}
