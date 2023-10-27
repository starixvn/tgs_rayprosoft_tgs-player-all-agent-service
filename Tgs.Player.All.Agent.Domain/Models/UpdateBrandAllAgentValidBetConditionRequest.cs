using MediatR;
using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public record UpdateBrandAllAgentValidBetConditionRequest(Guid BrandId,
    ValidBetConditionViewModel[] Conditions,
    AllAgentGameType AllAgentGameType) : IRequest;