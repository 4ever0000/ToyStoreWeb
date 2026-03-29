namespace ToyStore.Frontend.Models;

public class CouponDto
{
    public int Id { get; set; }
    public string Code { get; set; }
    public decimal DiscountAmount { get; set; }
    public int DiscountPercentage { get; set; }
    public DateTime ExpiryDate { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public bool IsActive { get; set; }
}

public class ApplyCouponDto
{
    public string CouponCode { get; set; }
}
