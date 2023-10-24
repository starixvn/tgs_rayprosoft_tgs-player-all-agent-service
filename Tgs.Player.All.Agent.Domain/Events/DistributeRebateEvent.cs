using MediatR;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Domain.Events;

public record DistributeRebateEvent(PlayerAllAgentRebateContributorModel PlayerAllAgentRebateContributor) : INotification;