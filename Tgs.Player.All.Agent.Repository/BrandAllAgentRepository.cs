using Microsoft.EntityFrameworkCore;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;
using Tgs.Player.All.Agent.Repository.Entities;

namespace Tgs.Player.All.Agent.Repository;

public class BrandAllAgentRepository : IBrandAllAgentRepository
{
    private readonly tgsContext _tgsContext;

    public BrandAllAgentRepository(tgsContext tgsContext)
    {
        _tgsContext = tgsContext;
    }

    public async Task UpdateSwitchAsync(Guid brandId, bool @switch, CancellationToken cancellationToken)
    {
        var isExists = await _tgsContext.BrandAllAgentSettings.AnyAsync(x => x.BrandId == brandId, cancellationToken)
            .ConfigureAwait(false);

        if (!isExists)
            await _tgsContext.BrandAllAgentSettings.AddAsync(new BrandAllAgentSetting
                    {
                        BrandId = brandId,
                        Switch = @switch
                    },
                    cancellationToken)
                .ConfigureAwait(false);
        else
            await _tgsContext.BrandAllAgentSettings.Where(x => x.BrandId == brandId)
                .ExecuteUpdateAsync(x => x.SetProperty(s => s.Switch, @switch), cancellationToken)
                .ConfigureAwait(false);

        await _tgsContext.SaveChangesAsync(cancellationToken);
    }

    public Task<bool> CheckGameTypeExists(Guid brandId,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken)
    {
        return _tgsContext.BrandAllAgentGameTypes.AnyAsync(x
                => x.BrandId == brandId && x.AllAgentGameType == (short)allAgentGameType,
            cancellationToken);
    }

    public async Task CreateGameTypesAsync(Guid brandId,
        bool @switch,
        bool hasValidBet,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken)
    {
        await _tgsContext.BrandAllAgentGameTypes.AddAsync(
            new BrandAllAgentGameType
            {
                BrandId = brandId,
                AllAgentGameType = (short)allAgentGameType,
                Switch = @switch,
                HasValidBet = hasValidBet
            },
            cancellationToken);

        await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateGameTypesAsync(Guid brandId,
        bool @switch,
        bool hasValidBet,
        AllAgentGameType allAgentGameType,
        CancellationToken cancellationToken)
    {
        await _tgsContext.BrandAllAgentGameTypes
            .Where(x => x.BrandId == brandId && x.AllAgentGameType == (short)allAgentGameType)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.Switch, @switch).SetProperty(s => s.HasValidBet, hasValidBet),
                cancellationToken)
            .ConfigureAwait(false);

        await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateSettleIntervalAsync(Guid brandId,
        SettlementIntervalType settlementIntervalType,
        CancellationToken cancellationToken)
    {
        await _tgsContext.BrandAllAgentSettings.Where(x => x.BrandId == brandId)
            .ExecuteUpdateAsync(x => x.SetProperty(s => s.SettlementIntervalType, (short)settlementIntervalType),
                cancellationToken)
            .ConfigureAwait(false);

        await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task UpdateValidBetConditionAsync(Guid brandId,
        AllAgentGameType allAgentGameType,
        ValidBetConditionViewModel[] conditions,
        CancellationToken cancellationToken)
    {
        await _tgsContext.BrandAllAgentGameTypeConditions.Where(x => x.BrandId == brandId &&
                                                                     x.AllAgentGameType == (short)allAgentGameType &&
                                                                     x.AllAgentRebateType ==
                                                                     (short)AllAgentRebateType.ValidBet)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);


        await _tgsContext.BrandAllAgentGameTypeConditions.AddRangeAsync(conditions.Select(x
                    => new BrandAllAgentGameTypeCondition
                    {
                        Rank = x.Rank,
                        BrandId = brandId,
                        NetProfit = x.NetProfit,
                        ActiveNumber = x.ActiveNumber,
                        RebatePercentage = x.RebatePercentage,
                        AllAgentGameType = (short)allAgentGameType,
                        AllAgentRebateType = (short)AllAgentRebateType.ValidBet
                    }),
                cancellationToken)
            .ConfigureAwait(false);

        await _tgsContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task<BrandAllAgentViewModel> GetBrandAllAgentAsync(Guid brandId, CancellationToken cancellationToken)
    {
        var brandAllAgent = await _tgsContext.BrandAllAgentSettings.Where(x => x.BrandId == brandId)
            .FirstOrDefaultAsync(cancellationToken);

        return new BrandAllAgentViewModel(brandAllAgent?.Switch ?? false,
            (SettlementIntervalType?)brandAllAgent?.SettlementIntervalType);
    }

    public IAsyncEnumerable<BrandAllAgentGameTypeViewModel> GetBrandAllAgentGameTypesAsync(Guid brandId)
    {
        return _tgsContext.BrandAllAgentGameTypes.Where(x => x.BrandId == brandId)
            .Select(x => new BrandAllAgentGameTypeViewModel
            {
                Switch = x.Switch,
                HasValidBet = x.HasValidBet,
                AllAgentGameType = (AllAgentGameType)x.AllAgentGameType
            })
            .ToAsyncEnumerable();
    }

    public IAsyncEnumerable<ValidBetConditionViewModel> GetBrandAllAgentValidBetConditionsAsync(Guid brandId,
        AllAgentGameType allAgentGameType)
    {
        return _tgsContext.BrandAllAgentGameTypeConditions
            .Where(x => x.BrandId == brandId &&
                        x.AllAgentGameType == (short)allAgentGameType &&
                        x.AllAgentRebateType == (short)AllAgentRebateType.ValidBet)
            .Select(x => new ValidBetConditionViewModel
            {
                Rank = x.Rank,
                ActiveNumber = x.ActiveNumber,
                NetProfit = x.NetProfit,
                RebatePercentage = x.RebatePercentage
            })
            .ToAsyncEnumerable();
    }
}