namespace ToyStore.Frontend.Models;

public class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime CreatedAt { get; set; } // Backend-də Created_at olsa belə, bura uyğunlaşdırırıq
}