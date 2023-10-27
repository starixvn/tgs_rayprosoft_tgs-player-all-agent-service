using MediatR;

namespace Tgs.Player.All.Agent.Domain.Models;

public record GetBrandAllAgentRequest(Guid BrandId) : IRequest<BrandAllAgentViewModel>;