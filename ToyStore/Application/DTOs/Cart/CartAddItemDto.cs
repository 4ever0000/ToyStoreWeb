using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Cart;

public class CartAddItemDto
{
    [Required]
    public int UserId { get; set; }
    [Required]
    public int ProductId { get; set; }
    [Range(1, 50)]
    public int Quantity { get; set; } = 1;
}
