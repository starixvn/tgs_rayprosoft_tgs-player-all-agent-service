using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public class PlayerAllAgentRebateModel
{
    public Guid BrandId { get; set; }

    public string PlayerCode { get; set; } = default!;

    public AllAgentGameType AllAgentGameType { get; set; }

    public AllAgentRebateType AllAgentRebateType { get; set; }

    public int Rank { get; set; }

    public int ActiveNumber { get; set; }

    public decimal ConditionAmount { get; set; }

    public decimal RebatePercentage { get; set; }

    public int ActualPlayerNumber { get; set; }

    public decimal ActualConditionAmount { get; set; }

    public decimal RebateAmount { get; set; }
}