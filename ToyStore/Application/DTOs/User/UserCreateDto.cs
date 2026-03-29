using System.ComponentModel.DataAnnotations;

namespace ToyStore.Application.DTOs.User;

public class UserCreateDto
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; }

    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(6)]
    public string Password { get; set; }

    [Phone]
    public string Phone { get; set; }
}
