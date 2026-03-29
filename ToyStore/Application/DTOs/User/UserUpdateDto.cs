using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.User;

public class UserUpdateDto
{
    [Required]
    public int Id { get; set; }

    [MinLength(2), MaxLength(100)]
    public string Name { get; set; }

    [Phone]
    public string Phone { get; set; }
}
