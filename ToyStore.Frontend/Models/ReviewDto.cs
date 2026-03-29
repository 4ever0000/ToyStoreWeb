namespace ToyStore.Frontend.Models;

public class ReviewDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; } // 1-5
    public DateTime CreatedAt { get; set; }
    public bool IsVerifiedPurchase { get; set; }
}

public class CreateReviewDto
{
    public int ProductId { get; set; }
    public string UserName { get; set; }
    public string Comment { get; set; }
    public int Rating { get; set; }
}
