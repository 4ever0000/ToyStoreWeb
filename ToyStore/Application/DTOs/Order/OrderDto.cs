namespace ToyStore.Application.DTOs.Order;

public class OrderDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingAddress { get; set; }
    public DateTime CreatedAt { get; set; }
}
