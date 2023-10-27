using MediatR;
using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public record CreatePlayerAllAgentDistributionRequest(SettlementIntervalType SettlementIntervalType,
    DateTime From,
    DateTime To) : IRequest;