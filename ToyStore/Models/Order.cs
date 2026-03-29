namespace ToyStore.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
