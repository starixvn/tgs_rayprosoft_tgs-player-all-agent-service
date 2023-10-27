namespace Tgs.Player.All.Agent.Client.Models;

public record GameTypeModel
{
    public bool Switch { get; set; }

    public bool HasValidBet { get; set; }

    public AllAgentGameType AllAgentGameType { get; set; }
}