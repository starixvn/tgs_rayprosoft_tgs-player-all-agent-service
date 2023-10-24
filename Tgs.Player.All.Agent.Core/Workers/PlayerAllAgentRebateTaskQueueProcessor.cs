using MediatR;
using Microsoft.Extensions.Logging;
using TGS.Core.Infrastructures.Extensions;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core.Workers;

public interface IPlayerAllAgentRebateTaskQueueProcessor
{
    Task ProcessQueueAsync(CancellationToken cancellationToken);
}

public class PlayerAllAgentRebateTaskQueueProcessor : IPlayerAllAgentRebateTaskQueueProcessor
{
    private readonly ILogger<PlayerAllAgentRebateTaskQueueProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly IPlayerAllAgentRepository _playerAllAgentRepository;
    private readonly IPlayerAllAgentRebateTaskQueue _taskQueue;

    public PlayerAllAgentRebateTaskQueueProcessor(
        IMediator mediator,
        IPlayerAllAgentRebateTaskQueue taskQueue,
        IPlayerAllAgentRepository playerAllAgentRepository,
        ILogger<PlayerAllAgentRebateTaskQueueProcessor> logger)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _playerAllAgentRepository = playerAllAgentRepository;
        _mediator = mediator;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        var request = await _taskQueue.DequeueAsync(cancellationToken);

        _logger.LogInformation("player all_agent_rebate request: {request}", request);

        var playerAggregate =
            _playerAllAgentRepository.GetPlayerAllAgentTurnoverWinLossAsync(request.From,
                request.To,
                request.ParentPlayerCode);

        var playerRebates = GetRebates(playerAggregate.ToEnumerable(), request.Conditions)
            .ToArray();

        if (await _playerAllAgentRepository.CheckRecordExistsAsync(request.ParentPlayerCode,
                request.From,
                request.To,
                cancellationToken))
        {
            _logger.LogInformation("save the all_agent_rebate already. request: {request}", request);

            return;
        }

        await _playerAllAgentRepository.CreatePlayerAllAgentRebateRecordsAsync(request.From,
            request.To,
            playerRebates,
            cancellationToken);

        var childPlayers = GetChildPlayers(playerAggregate.ToEnumerable(), playerRebates);

        await _playerAllAgentRepository.CreatePlayerAllAgentRebateContributorsAsync(request.From,
            request.To,
            childPlayers,
            cancellationToken);

        foreach (var childPlayer in childPlayers)
        {
            await _mediator.Publish(new CreatePlayerBucketEvent(childPlayer), cancellationToken);

            await _playerAllAgentRepository.SetDistributedAsync(childPlayer.Id, cancellationToken);

            await _mediator.Publish(new DistributeRebateEvent(childPlayer), cancellationToken);
        }
    }

    private static bool MatchConditionSetting(PlayerAllAgentTurnoverWinLossPlayerNumberModel model,
        IEnumerable<BrandAllAgentGameTypeConditionModel> conditions)
    {
        return conditions.Any(x => model.BrandId == x.BrandId &&
                                   model.AllAgentGameType == x.AllAgentGameType);
    }

    private static PlayerAllAgentRebateModel CheckRebate(
        PlayerAllAgentTurnoverWinLossPlayerNumberModel model,
        IEnumerable<BrandAllAgentGameTypeConditionModel> gameTypeConditions)
    {
        var conditions = gameTypeConditions.Where(x => model.BrandId == x.BrandId &&
                                                       model.AllAgentGameType == x.AllAgentGameType)
            .ToArray();

        var validBetConditions = conditions.Where(x
                => x is { HasValidBet: true, AllAgentRebateType: AllAgentRebateType.ValidBet })
            .ToArray();

        var allAgentGameType = model.AllAgentGameType;

        var validBetRebateCondition = validBetConditions.Where(x => x.AllAgentGameType == allAgentGameType)
            .OrderByDescending(x => x.Rank)
            .FirstOrDefault(x => model.PlayerNumber >= x.ActiveNumber &&
                                 model.Turnover >= x.NetProfit);

        var actualTurnoverAmount = model.Turnover;

        return new PlayerAllAgentRebateModel
        {
            BrandId = model.BrandId,
            PlayerCode = model.PlayerCode,
            AllAgentGameType = allAgentGameType,
            AllAgentRebateType = AllAgentRebateType.ValidBet,
            Rank = validBetRebateCondition?.Rank ?? 0,
            ActiveNumber = validBetRebateCondition?.ActiveNumber ?? 0,
            ConditionAmount = validBetRebateCondition?.NetProfit ?? 0M,
            RebatePercentage = validBetRebateCondition?.RebatePercentage ?? 0M,
            ActualPlayerNumber = model.PlayerNumber,
            ActualConditionAmount = actualTurnoverAmount,
            RebateAmount = actualTurnoverAmount * (validBetRebateCondition?.RebatePercentage ?? 0M) / 100
        };
    }

    private static IEnumerable<PlayerAllAgentRebateModel> GetRebates(
        IEnumerable<PlayerAllAgentTurnoverWinLossModel> models,
        IEnumerable<BrandAllAgentGameTypeConditionModel> gameTypeConditions)
    {
        var playerTurnovers =
            models
                .Select(x => new
                {
                    x.BrandId,
                    x.PlayerCode,
                    x.ChildPlayerCode,
                    x.Turnover,
                    x.WinLoss,
                    AllAgentGameType = GetAllAgentGameType(x.GameType)
                })
                .GroupBy(x => new { x.BrandId, x.PlayerCode, x.AllAgentGameType },
                    (x, list) => new
                    {
                        x.BrandId,
                        x.PlayerCode,
                        x.AllAgentGameType,
                        list
                    })
                .Select(x => new PlayerAllAgentTurnoverWinLossPlayerNumberModel
                {
                    BrandId = x.BrandId,
                    PlayerCode = x.PlayerCode,
                    AllAgentGameType = x.AllAgentGameType,
                    PlayerNumber =
                        x.list.Select(l => l.ChildPlayerCode).Distinct().Count(),
                    Turnover =
                        x.list.Sum(a => a.Turnover),
                    WinLoss =
                        x.list.Sum(a => a.WinLoss)
                });

        return playerTurnovers
            .Where(x => MatchConditionSetting(x, gameTypeConditions))
            .Select(x => CheckRebate(x, gameTypeConditions));
    }

    private static IEnumerable<PlayerAllAgentRebateContributorModel>
        GetChildPlayers(
            IEnumerable<PlayerAllAgentTurnoverWinLossModel> models,
            IEnumerable<PlayerAllAgentRebateModel> playerRebates)
    {
        return models.Select(x =>
            {
                var allAgentGameType = GetAllAgentGameType(x.GameType);

                var turnoverCondition = playerRebates.FirstOrDefault(p
                    => p.AllAgentGameType == allAgentGameType &&
                       p.AllAgentRebateType == AllAgentRebateType.ValidBet);

                var winLossCondition = playerRebates.FirstOrDefault(p
                    => p.AllAgentGameType == allAgentGameType &&
                       p.AllAgentRebateType == AllAgentRebateType.WinLoss);

                return new
                {
                    x.BrandId,
                    x.PlayerCode,
                    x.ChildPlayerCode,
                    SportsTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.Sportsbook
                        ? x.Turnover
                        : 0M,
                    VirtualSportsTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.VirtualSports
                        ? x.Turnover
                        : 0M,
                    LiveDealerTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.LiveDealer
                        ? x.Turnover
                        : 0M,
                    CardGameTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.CardGame
                        ? x.Turnover
                        : 0M,
                    SlotTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.Slot ? x.Turnover : 0M,
                    LotteryTurnover = turnoverCondition?.AllAgentGameType == AllAgentGameType.Lottery ? x.Turnover : 0M,
                    SportsWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.Sportsbook ? x.WinLoss : 0M,
                    VirtualSportsWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.VirtualSports
                        ? x.WinLoss
                        : 0M,
                    LiveDealerWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.LiveDealer
                        ? x.WinLoss
                        : 0M,
                    CardGameWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.CardGame ? x.WinLoss : 0M,
                    SlotWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.Slot ? x.WinLoss : 0M,
                    LotteryWinLoss = turnoverCondition?.AllAgentGameType == AllAgentGameType.Lottery ? x.WinLoss : 0M,
                    SportsTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.Sportsbook
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    VirtualSportsTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.VirtualSports
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    LiveDealerTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.LiveDealer
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    CardGameTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.CardGame
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    SlotTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.Slot
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    LotteryTurnoverRebate = turnoverCondition?.AllAgentGameType == AllAgentGameType.Lottery
                        ? (turnoverCondition?.RebatePercentage ?? 0M) / 100 * x.Turnover
                        : 0M,
                    SportsWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.Sportsbook
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M,
                    VirtualSportsWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.VirtualSports
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M,
                    LiveDealerWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.LiveDealer
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M,
                    CardGameWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.CardGame
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M,
                    SlotWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.Slot
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M,
                    LotteryWinLossRebate = winLossCondition?.AllAgentGameType == AllAgentGameType.Lottery
                        ? (winLossCondition?.RebatePercentage ?? 0M) / 100 * x.WinLoss
                        : 0M
                };
            })
            .GroupBy(x => new { x.BrandId, x.PlayerCode, x.ChildPlayerCode },
                (child, list) => new PlayerAllAgentRebateContributorModel
                {
                    Id = GuidExtensions.NewSequentialGuid(),
                    BrandId = child.BrandId,
                    PlayerCode = child.PlayerCode,
                    ChildPlayerCode = child.ChildPlayerCode,
                    SportsTurnover = list.Sum(s => s.SportsTurnover),
                    VirtualSportsTurnover = list.Sum(s => s.VirtualSportsTurnover),
                    LiveDealerTurnover = list.Sum(s => s.LiveDealerTurnover),
                    CardGameTurnover = list.Sum(s => s.CardGameTurnover),
                    SlotTurnover = list.Sum(s => s.SlotTurnover),
                    LotteryTurnover = list.Sum(s => s.LotteryTurnover),
                    SportsWinLoss = list.Sum(s => s.SportsWinLoss),
                    VirtualSportsWinLoss = list.Sum(s => s.VirtualSportsWinLoss),
                    LiveDealerWinLoss = list.Sum(s => s.LiveDealerWinLoss),
                    CardGameWinLoss = list.Sum(s => s.CardGameWinLoss),
                    SlotWinLoss = list.Sum(s => s.SlotWinLoss),
                    LotteryWinLoss = list.Sum(s => s.LotteryWinLoss),
                    SportsTurnoverRebate = list.Sum(s => s.SportsTurnoverRebate),
                    VirtualSportsTurnoverRebate = list.Sum(s => s.VirtualSportsTurnoverRebate),
                    LiveDealerTurnoverRebate = list.Sum(s => s.LiveDealerTurnoverRebate),
                    CardGameTurnoverRebate = list.Sum(s => s.CardGameTurnoverRebate),
                    SlotTurnoverRebate = list.Sum(s => s.SlotTurnoverRebate),
                    LotteryTurnoverRebate = list.Sum(s => s.LotteryTurnoverRebate),
                    SportsWinLossRebate = list.Sum(s => s.SportsWinLossRebate),
                    VirtualSportsWinLossRebate = list.Sum(s => s.VirtualSportsWinLossRebate),
                    LiveDealerWinLossRebate = list.Sum(s => s.LiveDealerWinLossRebate),
                    CardGameWinLossRebate = list.Sum(s => s.CardGameWinLossRebate),
                    SlotWinLossRebate = list.Sum(s => s.SlotWinLossRebate),
                    LotteryWinLossRebate = list.Sum(s => s.LotteryWinLossRebate)
                });
    }

    private static AllAgentGameType GetAllAgentGameType(GameType gameType)
    {
        return gameType switch
        {
            GameType.Sportsbook or GameType.ESports or GameType.VirtualSportsLobby => AllAgentGameType.Sportsbook,
            GameType.VirtualSports => AllAgentGameType.VirtualSports,
            GameType.LiveDealer or GameType.LiveDealerLobby => AllAgentGameType.LiveDealer,
            GameType.CardGame or GameType.CardGameLobby => AllAgentGameType.CardGame,
            GameType.Fishing or GameType.FishingLobby or GameType.Arcade or GameType.ArcadeLobby or GameType.Slot or GameType.TableGame => AllAgentGameType
                .Slot,
            GameType.Lottery or GameType.LotteryLobby => AllAgentGameType.Lottery,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}