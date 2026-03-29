using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Cart;

public class CartUpdateQuantityDto
{
    [Required]
    public int Id { get; set; }
    [Range(1, 50)]
    public int Quantity { get; set; }
}
