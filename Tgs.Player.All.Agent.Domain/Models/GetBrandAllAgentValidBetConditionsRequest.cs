using MediatR;
using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public record GetBrandAllAgentValidBetConditionsRequest
    (Guid BrandId, AllAgentGameType AllAgentGameType) : IStreamRequest<ValidBetConditionViewModel>;