using MediatR;
using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public record UpdateBrandAllAgentGameTypeSwitchRequest(Guid BrandId,
    bool Switch,
    bool HasValidBet,
    AllAgentGameType AllAgentGameType) : IRequest;