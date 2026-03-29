namespace ToyStore.Frontend.Models;

public class CartAddItemDto
{
    public int UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
