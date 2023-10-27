namespace Tgs.Player.All.Agent.Client.Models;

public record UpdateValidBetConditionRequest(Guid BrandId,
    ValidBetConditionModel[] Conditions,
    AllAgentGameType AllAgentGameType);