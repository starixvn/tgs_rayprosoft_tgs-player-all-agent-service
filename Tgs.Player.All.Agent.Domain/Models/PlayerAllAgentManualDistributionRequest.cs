using MediatR;

namespace Tgs.Player.All.Agent.Domain.Models;

public record PlayerAllAgentManualDistributionRequest(DateTime From, DateTime To) : IRequest;