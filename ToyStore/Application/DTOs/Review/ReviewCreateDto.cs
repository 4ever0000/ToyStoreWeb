using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Review;

public class ReviewCreateDto
{
    [Required]
    public int ProductId { get; set; }
    [Required]
    public int UserId { get; set; }

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(1000)]
    public string Comment { get; set; }
}
