using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Domain.Interfaces;

public interface IBrandAllAgentRepository
{
    Task UpdateSwitchAsync(Guid brandId,
        bool @switch,
        CancellationToken cancellationToken);

    Task<bool> CheckGameTypeExists(Guid brandId,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken);

    Task CreateGameTypesAsync(Guid brandId,
        bool @switch,
        bool hasValidBet,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken);

    Task UpdateGameTypesAsync(Guid brandId,
        bool @switch,
        bool hasValidBet,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken);

    Task UpdateSettleIntervalAsync(Guid brandId,
        SettlementIntervalType settlementIntervalType,
        CancellationToken cancellationToken);

    Task UpdateValidBetConditionAsync(Guid brandId,
        AllAgentGameType allAgentGameType,
        ValidBetConditionViewModel[] conditions,
        CancellationToken cancellationToken);

    Task<BrandAllAgentViewModel> GetBrandAllAgentAsync(Guid brandId, CancellationToken cancellationToken);

    IAsyncEnumerable<BrandAllAgentGameTypeViewModel> GetBrandAllAgentGameTypesAsync(Guid brandId);

    IAsyncEnumerable<ValidBetConditionViewModel> GetBrandAllAgentValidBetConditionsAsync(Guid brandId,
        AllAgentGameType allAgentGameType);
}