using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Brand;

public class BrandCreateDto
{
    [Required(ErrorMessage = "Brend adı boş ola bilməz")]
    [MinLength(2)]
    [MaxLength(100)]
    public string Name { get; set; }
}
