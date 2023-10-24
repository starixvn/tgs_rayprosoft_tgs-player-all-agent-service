namespace Tgs.Player.All.Agent.Domain.Models;

public record PlayerAllAgentRebateQueueModel
{
    public string ParentPlayerCode { get; init; } = default!;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public IEnumerable<BrandAllAgentGameTypeConditionModel> Conditions { get; init; } = default!;
}