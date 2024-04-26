namespace Tgs.Player.All.Agent.Client.Models;

public class ValidBetConditionModel
{
    public AllAgentGameType AllAgentGameType { get; set; }

    public int ActiveNumber { get; set; }

    public decimal NetProfit { get; set; }

    public int Rank { get; set; }

    public decimal RebatePercentage { get; set; }
}