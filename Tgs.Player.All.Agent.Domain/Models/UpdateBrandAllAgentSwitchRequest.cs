using MediatR;

namespace Tgs.Player.All.Agent.Domain.Models;

public record UpdateBrandAllAgentSwitchRequest(Guid BrandId, bool Switch) : IRequest;