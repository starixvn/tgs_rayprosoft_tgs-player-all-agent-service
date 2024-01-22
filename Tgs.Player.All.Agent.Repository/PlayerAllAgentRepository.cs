using Microsoft.EntityFrameworkCore;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;
using Tgs.Player.All.Agent.Repository.Entities;

namespace Tgs.Player.All.Agent.Repository;

public class PlayerAllAgentRepository : IPlayerAllAgentRepository
{
    private readonly tgsContext _tgsContext;

    public PlayerAllAgentRepository(tgsContext tgsContext)
    {
        _tgsContext = tgsContext;
    }

    public IAsyncEnumerable<ValidPlayer> GetValidPlayersAsync(DateTime from,
        DateTime to,
        SettlementIntervalType settlementIntervalType)
    {
        return _tgsContext.BrandAllAgentSettings.Where(x =>
                x.SettlementIntervalType == (short)settlementIntervalType && x.Switch)
            .Join(
                _tgsContext.PlayerGameRebateAggregates
                    .Where(x => x.ReportDate >= from && x.ReportDate <= to),
                brand => brand.BrandId,
                aggregate => aggregate.BrandId,
                (brandAllAgent, aggregate) => new
                {
                    Aggregate = aggregate,
                    BrandAllAgent = brandAllAgent
                })
            .GroupJoin(_tgsContext.BrandGames,
                group => new
                {
                    group.Aggregate.BrandId,
                    group.Aggregate.GameId
                },
                brandGame => new
                {
                    brandGame.BrandId,
                    brandGame.GameId
                },
                (aggregate, brandGame) => new
                {
                    aggregate.Aggregate,
                    BrandGame = brandGame
                })
            .SelectMany(x => x.BrandGame.DefaultIfEmpty(),
                (x, brandGame) => new
                {
                    x.Aggregate,
                    BrandGame = brandGame
                })
            .Where(x => x.BrandGame != null && x.BrandGame.Rebate)
            .Join(_tgsContext.PlayerParents,
                group => group.Aggregate.PlayerCode,
                player => player.PlayerCode,
                (group, player) => new
                {
                    group.Aggregate,
                    group.BrandGame,
                    Player = player
                })
            .GroupBy(
                x => new
                {
                    x.Aggregate.BrandId,
                    x.Player.ParentPlayerCode
                },
                (x, list) => new
                {
                    x.BrandId,
                    x.ParentPlayerCode
                })
            .Select(x => new ValidPlayer(x.BrandId, x.ParentPlayerCode))
            .ToAsyncEnumerable();
    }

    public IAsyncEnumerable<BrandAllAgentGameTypeConditionViewModel> GetBrandAllAgentGameTypeConditionsAsync(
        Guid[] brandIds)
    {
        return _tgsContext.BrandAllAgentSettings
            .Where(x => x.Switch)
            .Join(
                _tgsContext.BrandAllAgentGameTypes,
                brand => brand.BrandId,
                gameType => gameType.BrandId,
                (brand, gameType) => new { brand.BrandId, GameType = gameType })
            .Join(_tgsContext.BrandAllAgentGameTypeConditions,
                group => new { group.BrandId, AllAgentGameType = (int)group.GameType.AllAgentGameType },
                condition => new { condition.BrandId, condition.AllAgentGameType },
                (group, condition) => new { group.BrandId, group.GameType, Condition = condition })
            .Where(x => brandIds.Contains(x.BrandId))
            .Where(x => x.GameType.Switch)
            .Where(x => x.GameType.HasValidBet || x.GameType.HasWinLoss)
            .Select(x => new BrandAllAgentGameTypeConditionViewModel
            {
                BrandId = x.BrandId,
                GameTypeSwitch = x.GameType.Switch,
                HasValidBet = x.GameType.HasValidBet,
                HasWinLoss = x.GameType.HasWinLoss,
                AllAgentGameType = (AllAgentGameType)x.GameType.AllAgentGameType,
                AllAgentRebateType = (AllAgentRebateType)x.Condition.AllAgentRebateType,
                Rank = x.Condition.Rank,
                ActiveNumber = x.Condition.ActiveNumber,
                NetProfit = x.Condition.NetProfit,
                RebatePercentage = x.Condition.RebatePercentage
            })
            .ToAsyncEnumerable();
    }

    public IAsyncEnumerable<PlayerAllAgentRebateContributorModel> GetNotSentRebatesAsync(DateTime from,
        DateTime to)
    {
        return _tgsContext.PlayerAllAgentRebateContributors
            .Where(x => x.From == from && x.To == to && x.SentOn == null)
            .Select(x => new PlayerAllAgentRebateContributorModel
            {
                Id = x.Id,
                BrandId = x.BrandId,
                PlayerCode = x.PlayerCode,
                ChildPlayerCode = x.ChildPlayerCode,
                SportsTurnover = x.SportsTurnover,
                VirtualSportsTurnover = x.VirtualSportsTurnover,
                LiveDealerTurnover = x.LiveDealerTurnover,
                CardGameTurnover = x.CardGameTurnover,
                SlotTurnover = x.SlotTurnover,
                LotteryTurnover = x.LotteryTurnover,
                SportsWinLoss = x.SportsWinLoss,
                VirtualSportsWinLoss = x.VirtualSportsWinLoss,
                LiveDealerWinLoss = x.LiveDealerWinLoss,
                CardGameWinLoss = x.CardGameWinLoss,
                SlotWinLoss = x.SlotWinLoss,
                LotteryWinLoss = x.LotteryWinLoss,
                SportsTurnoverRebate = x.SportsTurnoverRebate,
                VirtualSportsTurnoverRebate = x.VirtualSportsTurnoverRebate,
                LiveDealerTurnoverRebate = x.LiveDealerTurnoverRebate,
                CardGameTurnoverRebate = x.CardGameTurnoverRebate,
                SlotTurnoverRebate = x.SlotTurnoverRebate,
                LotteryTurnoverRebate = x.LotteryTurnoverRebate,
                SportsWinLossRebate = x.SportsWinLossRebate,
                VirtualSportsWinLossRebate = x.VirtualSportsWinLossRebate,
                LiveDealerWinLossRebate = x.LiveDealerWinLossRebate,
                CardGameWinLossRebate = x.CardGameWinLossRebate,
                SlotWinLossRebate = x.SlotWinLossRebate,
                LotteryWinLossRebate = x.LotteryWinLossRebate
            })
            .ToAsyncEnumerable();
    }

    public IAsyncEnumerable<PlayerAllAgentTurnoverWinLossModel> GetPlayerAllAgentTurnoverWinLossAsync(
        DateTime from,
        DateTime to,
        string parentPlayerCode)
    {
        return _tgsContext
            .PlayerGameRebateAggregates
            .Where(x => x.ReportDate >= from && x.ReportDate <= to.AddDays(1))
            .Join(_tgsContext.PlayerParents,
                group => group.PlayerCode,
                player => player.PlayerCode,
                (group, player) => new { Aggregate = group, Player = player })
            .Where(x => x.Player.ParentPlayerCode == parentPlayerCode)
            .GroupJoin(_tgsContext.BrandGames,
                group => new
                {
                    group.Aggregate.BrandId,
                    group.Aggregate.GameId
                },
                brandGame => new
                {
                    brandGame.BrandId,
                    brandGame.GameId
                },
                (aggregate, brandGame) => new
                {
                    aggregate.Aggregate,
                    aggregate.Player,
                    BrandGame = brandGame
                })
            .SelectMany(x => x.BrandGame.DefaultIfEmpty(),
                (x, brandGame) => new
                {
                    x.Aggregate,
                    x.Player,
                    BrandGame = brandGame
                })
            .Where(x => x.BrandGame != null && x.BrandGame.Rebate)
            .GroupBy(x => new
                {
                    x.Aggregate.BrandId,
                    x.Player.ParentPlayerCode,
                    x.Aggregate.PlayerCode,
                    x.Aggregate.GameType
                },
                (x, list) => new PlayerAllAgentTurnoverWinLossModel
                {
                    BrandId = x.BrandId,
                    PlayerCode = x.ParentPlayerCode,
                    ChildPlayerCode = x.PlayerCode,
                    GameType = (GameType)x.GameType,
                    Turnover = list.Sum(t => t.Aggregate.Turnover),
                    WinLoss = list.Sum(a => a.Aggregate.WinLoss)
                })
            .ToAsyncEnumerable();
    }

    public async Task CreatePlayerAllAgentRebateRecordsAsync(DateTime from,
        DateTime to,
        IEnumerable<PlayerAllAgentRebateModel> models,
        CancellationToken cancellationToken)
    {
        await _tgsContext.PlayerAllAgentRebateRecords.AddRangeAsync(models.Select(model =>
                new PlayerAllAgentRebateRecord
                {
                    Id = Guid.NewGuid(),
                    BrandId = model.BrandId,
                    PlayerCode = model.PlayerCode,
                    SettleDate = to.AddDays(1),
                    From = from,
                    To = to,
                    CreatedOn = DateTime.UtcNow,
                    AllAgentGameType = (short)model.AllAgentGameType,
                    AllAgentRebateType = (short)model.AllAgentRebateType,
                    Rank = model.Rank,
                    ActiveNumber = model.ActiveNumber,
                    ConditionAmount = model.ConditionAmount,
                    RebatePercentage = model.RebatePercentage,
                    ActualConditionAmount = model.ActualConditionAmount,
                    ActualPlayerNumber = model.ActualPlayerNumber,
                    RebateAmount = model.RebateAmount
                })
            .ToArray());

        _ = await _tgsContext.SaveChangesAsync(cancellationToken);
    }

    public async Task CreatePlayerAllAgentRebateContributorsAsync(DateTime from,
        DateTime to,
        IEnumerable<PlayerAllAgentRebateContributorModel> models,
        CancellationToken cancellationToken)
    {
        await _tgsContext.PlayerAllAgentRebateContributors.AddRangeAsync(models.Select(model =>
                new PlayerAllAgentRebateContributor
                {
                    Id = model.Id,
                    BrandId = model.BrandId,
                    PlayerCode = model.PlayerCode,
                    ChildPlayerCode = model.ChildPlayerCode,
                    SettleDate = to.AddDays(1),
                    From = from,
                    To = to,
                    CreatedOn = DateTime.UtcNow,
                    SportsTurnover = model.SportsTurnover,
                    VirtualSportsTurnover = model.VirtualSportsTurnover,
                    LiveDealerTurnover = model.LiveDealerTurnover,
                    CardGameTurnover = model.CardGameTurnover,
                    SlotTurnover = model.SlotTurnover,
                    LotteryTurnover = model.LotteryTurnover,
                    SportsWinLoss = model.SportsWinLoss,
                    VirtualSportsWinLoss = model.VirtualSportsWinLoss,
                    LiveDealerWinLoss = model.LiveDealerWinLoss,
                    CardGameWinLoss = model.CardGameWinLoss,
                    SlotWinLoss = model.SlotWinLoss,
                    LotteryWinLoss = model.LotteryWinLoss,
                    SportsTurnoverRebate = model.SportsTurnoverRebate,
                    VirtualSportsTurnoverRebate = model.VirtualSportsTurnoverRebate,
                    LiveDealerTurnoverRebate = model.LiveDealerTurnoverRebate,
                    CardGameTurnoverRebate = model.CardGameTurnoverRebate,
                    SlotTurnoverRebate = model.SlotTurnoverRebate,
                    LotteryTurnoverRebate = model.LotteryTurnoverRebate,
                    SportsWinLossRebate = model.SportsWinLossRebate,
                    VirtualSportsWinLossRebate = model.VirtualSportsWinLossRebate,
                    LiveDealerWinLossRebate = model.LiveDealerWinLossRebate,
                    CardGameWinLossRebate = model.CardGameWinLossRebate,
                    SlotWinLossRebate = model.SlotWinLossRebate,
                    LotteryWinLossRebate = model.LotteryWinLossRebate
                }),
            cancellationToken);

        _ = await _tgsContext.SaveChangesAsync(cancellationToken);
    }

    public Task SetDistributedAsync(Guid id, CancellationToken cancellationToken)
    {
        return _tgsContext.PlayerAllAgentRebateContributors.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x =>
                    x.SetProperty(p => p.SentOn, p => DateTime.UtcNow),
                cancellationToken);
    }

    public Task<bool> CheckRecordExistsAsync(string playerCode,
        DateTime from,
        DateTime to,
        CancellationToken cancellationToken)
    {
        return _tgsContext.PlayerAllAgentRebateRecords
            .AnyAsync(
                x => x.PlayerCode == playerCode &&
                     x.From >= from &&
                     x.To <= to,
                cancellationToken);
    }

    public Task<bool> CheckTransactionByExternalIdExistsAsync(string playerCode,
        string externalId,
        CancellationToken cancellationToken)
    {
        return _tgsContext.Transactions.AnyAsync(
            x => x.PlayerCode == playerCode && x.ExternalTransactionId == externalId,
            cancellationToken);
    }

    public async Task<Guid> GetPlayerAllAgentRebateContributorIdAsync(DateTime from,
        DateTime to,
        string playerCode,
        string childPlayerCode,
        CancellationToken cancellationToken)
    {
        var playerAllAgentRebateContributor = await _tgsContext.PlayerAllAgentRebateContributors.FirstAsync(
            x => x.PlayerCode == playerCode && x.ChildPlayerCode == childPlayerCode && x.From == from && x.To == to,
            cancellationToken);

        return playerAllAgentRebateContributor.Id;
    }
}