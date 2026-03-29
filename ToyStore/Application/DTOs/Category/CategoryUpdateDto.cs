using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Category;

public class CategoryUpdateDto
{
    [Required]
    public int Id { get; set; }
    [Required, MinLength(2), MaxLength(150)]
    public string Name { get; set; }
    public int? ParentId { get; set; }
}
