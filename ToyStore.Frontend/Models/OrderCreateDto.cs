using System.ComponentModel.DataAnnotations;

namespace ToyStore.Frontend.Models
{
    public class OrderCreateDto
    {
        [Required(ErrorMessage = "İstifadəçi ID mütləqdir")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Çatdırılma ünvanı mütləqdir"), MinLength(10, ErrorMessage = "Ünvan ən azı 10 simvol olmalıdır")]
        public string ShippingAddress { get; set; }

        // Qeyd: API-ın böyük ehtimalla məhsulları da burada gözləyir. 
        // Əgər Backend-də yoxdursa, bunu ora əlavə etməli olacaqsan.
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}