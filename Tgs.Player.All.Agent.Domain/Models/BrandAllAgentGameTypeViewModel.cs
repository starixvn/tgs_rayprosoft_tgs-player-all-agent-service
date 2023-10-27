using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public record BrandAllAgentGameTypeViewModel
{
    public bool Switch { get; init; }

    public bool HasValidBet { get; init; }

    public AllAgentGameType AllAgentGameType { get; init; }
}