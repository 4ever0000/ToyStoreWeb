using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Order
{
    public class OrderCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required, MinLength(10)]
        public string ShippingAddress { get; set; }
    }
}
