using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Domain.Interfaces;

public interface IPlayerAllAgentRepository
{
    IAsyncEnumerable<ValidPlayer> GetValidPlayersAsync(DateTime from,
        DateTime to,
        SettlementIntervalType settlementIntervalType);

    IAsyncEnumerable<BrandAllAgentGameTypeConditionViewModel> GetBrandAllAgentGameTypeConditionsAsync(Guid[] brandIds);

    IAsyncEnumerable<PlayerAllAgentRebateContributorModel> GetNotSentRebatesAsync(DateTime from, DateTime to);

    IAsyncEnumerable<PlayerAllAgentTurnoverWinLossModel> GetPlayerAllAgentTurnoverWinLossAsync(DateTime from,
        DateTime to,
        string parentPlayerCode);

    Task CreatePlayerAllAgentRebateRecordsAsync(DateTime from,
        DateTime to,
        IEnumerable<PlayerAllAgentRebateModel> models,
        CancellationToken cancellationToken);

    Task CreatePlayerAllAgentRebateContributorsAsync(DateTime from,
        DateTime to,
        IEnumerable<PlayerAllAgentRebateContributorModel> models,
        CancellationToken cancellationToken);

    Task SetDistributedAsync(Guid id, CancellationToken cancellationToken);

    Task<bool> CheckRecordExistsAsync(string playerCode,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken);

    Task<bool> CheckTransactionByExternalIdExistsAsync(string playerCode,
        string externalId,
        CancellationToken cancellationToken);

    Task<Guid> GetPlayerAllAgentRebateContributorIdAsync(DateTime from,
        DateTime to,
        string playerCode,
        string childPlayerCode,
        CancellationToken cancellationToken);
}