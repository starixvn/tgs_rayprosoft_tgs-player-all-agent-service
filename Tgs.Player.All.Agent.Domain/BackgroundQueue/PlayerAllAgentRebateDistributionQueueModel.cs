using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Domain.BackgroundQueue;

public record PlayerAllAgentRebateDistributionQueueModel
{
    public required PlayerAllAgentRebateContributorModel Contributor { get; init; }
}