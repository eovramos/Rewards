using System.Text.Json.Serialization;

namespace Rewards.Models;

public class Customer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<Purchase> Purchases { get; set; } = new();

    [JsonIgnore]
    public int TotalPaidPurchases => Purchases.Count(p => !p.IsFreeRedemption);

    [JsonIgnore]
    public int RewardsEarned => TotalPaidPurchases / 10;

    [JsonIgnore]
    public int RewardsClaimed => Purchases.Count(p => p.IsFreeRedemption);

    [JsonIgnore]
    public int RewardsAvailable => RewardsEarned - RewardsClaimed;

    [JsonIgnore]
    public int StampProgress => TotalPaidPurchases % 10;
}
