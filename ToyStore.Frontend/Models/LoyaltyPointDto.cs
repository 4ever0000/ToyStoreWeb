namespace ToyStore.Frontend.Models;

public class LoyaltyPointDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public int SpentPoints { get; set; }
    public List<PointTransactionDto> Transactions { get; set; }
}

public class PointTransactionDto
{
    public int Id { get; set; }
    public int PointsAmount { get; set; }
    public string Description { get; set; } // "Siparış", "Şərh", "Referral", vb
    public DateTime CreatedAt { get; set; }
    public bool IsCredit { get; set; } // True: points added, False: points spent
}

public class PointsEarnedDto
{
    public int OrderId { get; set; }
    public int PointsEarned { get; set; }
    public string Reason { get; set; } // "Satın Almalar", "Şərh Yazma", vb
}
