using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Domain.BackgroundQueue;

public record PlayerAllAgentRebateQueueModel
{
    public string ParentPlayerCode { get; init; } = default!;
    public DateTime From { get; init; }
    public DateTime To { get; init; }
    public IEnumerable<BrandAllAgentGameTypeConditionViewModel> Conditions { get; init; } = default!;
}