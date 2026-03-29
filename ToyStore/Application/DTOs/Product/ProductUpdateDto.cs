using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.Product
{
    public class ProductUpdateDto : ProductCreateDto
    {
        [Required]
        public int Id { get; set; }
    }
}
