using Tgs.Player.All.Agent.Domain.Enums;

namespace Tgs.Player.All.Agent.Domain.Models;

public class PlayerAllAgentTurnoverWinLossModel
{
    public Guid BrandId { get; set; }

    public string PlayerCode { get; set; } = default!;

    public string ChildPlayerCode { get; set; } = default!;

    public GameType GameType { get; set; }

    public decimal Turnover { get; set; }

    public decimal WinLoss { get; set; }
}