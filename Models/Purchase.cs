namespace Rewards.Models;

public class Purchase
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Date { get; set; } = DateTime.Now;
    public bool IsFreeRedemption { get; set; } = false;
}
