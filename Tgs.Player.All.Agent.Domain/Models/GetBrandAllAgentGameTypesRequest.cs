using MediatR;

namespace Tgs.Player.All.Agent.Domain.Models;

public record GetBrandAllAgentGameTypesRequest(Guid BrandId) : IStreamRequest<BrandAllAgentGameTypeViewModel>;