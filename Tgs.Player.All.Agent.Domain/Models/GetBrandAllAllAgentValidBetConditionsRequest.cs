using MediatR;

namespace Tgs.Player.All.Agent.Domain.Models;

public record GetBrandAllAllAgentValidBetConditionsRequest
    (Guid BrandId) : IStreamRequest<ValidBetConditionViewModel>;