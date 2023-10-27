namespace Tgs.Player.All.Agent.Client.Models;

public record UpdateGameTypeSwitchRequest(Guid BrandId,
    bool Switch,
    bool HasValidBet,
    AllAgentGameType AllAgentGameType);