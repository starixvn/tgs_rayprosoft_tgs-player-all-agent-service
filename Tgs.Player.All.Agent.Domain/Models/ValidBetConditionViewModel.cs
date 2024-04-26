using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public class ValidBetConditionViewModel
{
    public AllAgentGameType AllAgentGameType { get; set; }

    public int ActiveNumber { get; init; }

    public decimal NetProfit { get; init; }

    public int Rank { get; init; }

    public decimal RebatePercentage { get; init; }
}