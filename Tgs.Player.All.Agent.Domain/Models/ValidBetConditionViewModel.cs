namespace Tgs.Player.All.Agent.Domain.Models;

public class ValidBetConditionViewModel
{
    public int ActiveNumber { get; init; }

    public decimal NetProfit { get; init; }

    public int Rank { get; init; }

    public decimal RebatePercentage { get; init; }
}