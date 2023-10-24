using AutoFixture;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.UnitTest.Workers;

public class PlayerAllAgentRebateTaskQueueProcessorTests
{
    [Test]
    public async Task Get_Player_All_Agent_Turnover_Win_Loss()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var fixture = new Fixture();
        var turnoverWinLosses = fixture.CreateMany<PlayerAllAgentTurnoverWinLossModel>(10);

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        var brandAllAgentGameTypeConditions = turnoverWinLosses.Select(x =>
            CreateValidBetConditionModelFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = brandAllAgentGameTypeConditions
            });

        await sut.ProcessQueueAsync(default);

        _ = fakePlayerAllAgentRepository.Received(1)
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>());
    }

    [Test]
    public async Task No_Create_Player_All_Agent_Distributions_When_Record_Existed()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var fixture = new Fixture();
        var turnoverWinLosses = fixture.CreateMany<PlayerAllAgentTurnoverWinLossModel>(10);

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.CheckRecordExistsAsync(Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(true);

        var brandAllAgentGameTypeConditions = turnoverWinLosses.Select(x =>
            CreateValidBetConditionModelFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = brandAllAgentGameTypeConditions
            });

        await sut.ProcessQueueAsync(default);

        _ = fakePlayerAllAgentRepository.DidNotReceiveWithAnyArgs()
            .CreatePlayerAllAgentRebateRecordsAsync(default, default, default!, default);

        _ = fakePlayerAllAgentRepository.DidNotReceiveWithAnyArgs()
            .CreatePlayerAllAgentRebateContributorsAsync(default, default, default!, default);
    }

    [Test]
    public async Task Create_Player_All_Agent_Distributions()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var fixture = new Fixture();
        var turnoverWinLosses = fixture.CreateMany<PlayerAllAgentTurnoverWinLossModel>(10);

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.CheckRecordExistsAsync(Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        var brandAllAgentGameTypeConditions = turnoverWinLosses.Select(x =>
            CreateValidBetConditionModelFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = brandAllAgentGameTypeConditions
            });

        await sut.ProcessQueueAsync(default);

        _ = fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateRecordsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<IEnumerable<PlayerAllAgentRebateModel>>(),
                Arg.Any<CancellationToken>());

        _ = fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateContributorsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<IEnumerable<PlayerAllAgentRebateContributorModel>>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Create_Player_All_Agent_Distributions_By_Card_Game_Turnover()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var brandId = Guid.NewGuid();

        var turnoverWinLosses = CreateTurnoverWinLossFixture(brandId, GameType.CardGame, 100)
            .CreateMany<PlayerAllAgentTurnoverWinLossModel>(1);

        var conditions = new BrandAllAgentGameTypeConditionModel[]
        {
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.CardGame,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 1,
                NetProfit = 1,
                RebatePercentage = 10,
                HasValidBet = true
            },
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 1,
                NetProfit = 2,
                RebatePercentage = 20,
                HasValidBet = true
            }
        };

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.CheckRecordExistsAsync(Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = conditions
            });

        await sut.ProcessQueueAsync(default);

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateRecordsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateModel>>(x => x.Any(a =>
                    a.RebateAmount == 10 &&
                    a.Rank == 1 &&
                    a.RebatePercentage == 10 &&
                    a.ActualPlayerNumber == 1 &&
                    a.ActualConditionAmount == 100)),
                Arg.Any<CancellationToken>());

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateContributorsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateContributorModel>>(x => x.Any(a =>
                    a.CardGameTurnover == 100 &&
                    a.CardGameTurnoverRebate == 10 &&
                    a.LiveDealerTurnoverRebate == 0 &&
                    a.LotteryTurnoverRebate == 0 &&
                    a.SlotTurnoverRebate == 0 &&
                    a.SportsTurnoverRebate == 0 &&
                    a.VirtualSportsTurnoverRebate == 0 &&
                    a.CardGameWinLossRebate == 0 &&
                    a.LiveDealerWinLossRebate == 0 &&
                    a.LotteryWinLossRebate == 0 &&
                    a.SlotWinLossRebate == 0 &&
                    a.SportsWinLossRebate == 0 &&
                    a.VirtualSportsWinLossRebate == 0)),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Create_Player_All_Agent_Distributions_By_Sports_Game_Turnover()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var brandId = Guid.NewGuid();

        var turnoverWinLosses = CreateTurnoverWinLossFixture(brandId, GameType.Sportsbook, 100)
            .CreateMany<PlayerAllAgentTurnoverWinLossModel>(1);

        var conditions = new BrandAllAgentGameTypeConditionModel[]
        {
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 1,
                ActiveNumber = 1,
                NetProfit = 1,
                RebatePercentage = 10,
                HasValidBet = true
            },
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 2,
                ActiveNumber = 1,
                NetProfit = 20,
                RebatePercentage = 20,
                HasValidBet = true
            }
        };

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.CheckRecordExistsAsync(Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = conditions
            });

        await sut.ProcessQueueAsync(default);

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateRecordsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateModel>>(x => x.Any(a =>
                    a.RebateAmount == 20 &&
                    a.Rank == 2 &&
                    a.RebatePercentage == 20 &&
                    a.ActualPlayerNumber == 1 &&
                    a.ActualConditionAmount == 100)),
                Arg.Any<CancellationToken>());

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateContributorsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateContributorModel>>(x => x.Any(a =>
                    a.CardGameTurnoverRebate == 0 &&
                    a.LiveDealerTurnoverRebate == 0 &&
                    a.LotteryTurnoverRebate == 0 &&
                    a.SlotTurnoverRebate == 0 &&
                    a.SportsTurnover == 100 &&
                    a.SportsTurnoverRebate == 20 &&
                    a.VirtualSportsTurnoverRebate == 0 &&
                    a.CardGameWinLossRebate == 0 &&
                    a.LiveDealerWinLossRebate == 0 &&
                    a.LotteryWinLossRebate == 0 &&
                    a.SlotWinLossRebate == 0 &&
                    a.SportsWinLossRebate == 0 &&
                    a.VirtualSportsWinLossRebate == 0)),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Create_Player_All_Agent_Distributions_For_Rank_3_Condition()
    {
        var fakeMediator = Substitute.For<IMediator>();
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new PlayerAllAgentRebateTaskQueueProcessor(
            fakeMediator,
            fakePlayerAllAgentRebateTaskQueue,
            fakePlayerAllAgentRepository,
            NullLogger<PlayerAllAgentRebateTaskQueueProcessor>.Instance);

        var brandId = Guid.NewGuid();

        var turnoverWinLosses = Enumerable.Range(0, 10)
            .Select(x =>
                CreateTurnoverWinLossFixture(brandId, GameType.Sportsbook, 100)
                    .Create<PlayerAllAgentTurnoverWinLossModel>());

        var conditions = new BrandAllAgentGameTypeConditionModel[]
        {
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 1,
                ActiveNumber = 1,
                NetProfit = 1,
                RebatePercentage = 5,
                HasValidBet = true
            },
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 2,
                ActiveNumber = 1,
                NetProfit = 20,
                RebatePercentage = 10,
                HasValidBet = true
            },
            new()
            {
                BrandId = brandId,
                AllAgentGameType = AllAgentGameType.Sportsbook,
                AllAgentRebateType = AllAgentRebateType.ValidBet,
                Rank = 3,
                ActiveNumber = 1,
                NetProfit = 100,
                RebatePercentage = 15,
                HasValidBet = true
            }
        };

        fakePlayerAllAgentRepository
            .GetPlayerAllAgentTurnoverWinLossAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<string>())
            .Returns(turnoverWinLosses.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.CheckRecordExistsAsync(Arg.Any<string>(),
                Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Any<CancellationToken>())
            .Returns(false);

        fakePlayerAllAgentRebateTaskQueue.DequeueAsync(Arg.Any<CancellationToken>())
            .Returns(new PlayerAllAgentRebateQueueModel
            {
                ParentPlayerCode = "playerCode",
                From = DateTime.UtcNow,
                To = DateTime.UtcNow,
                Conditions = conditions
            });

        await sut.ProcessQueueAsync(default);

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateRecordsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateModel>>(x => x.Any(a =>
                    a.RebateAmount == 15 &&
                    a.Rank == 3 &&
                    a.RebatePercentage == 15 &&
                    a.ActualPlayerNumber == 1 &&
                    a.ActualConditionAmount == 100)),
                Arg.Any<CancellationToken>());

        await fakePlayerAllAgentRepository.Received(1)
            .CreatePlayerAllAgentRebateContributorsAsync(Arg.Any<DateTime>(),
                Arg.Any<DateTime>(),
                Arg.Is<IEnumerable<PlayerAllAgentRebateContributorModel>>(x => x.Any(a =>
                    a.CardGameTurnoverRebate == 0 &&
                    a.LiveDealerTurnoverRebate == 0 &&
                    a.LotteryTurnoverRebate == 0 &&
                    a.SlotTurnoverRebate == 0 &&
                    a.SportsTurnover == 100 &&
                    a.SportsTurnoverRebate == 15 &&
                    a.VirtualSportsTurnoverRebate == 0 &&
                    a.CardGameWinLossRebate == 0 &&
                    a.LiveDealerWinLossRebate == 0 &&
                    a.LotteryWinLossRebate == 0 &&
                    a.SlotWinLossRebate == 0 &&
                    a.SportsWinLossRebate == 0 &&
                    a.VirtualSportsWinLossRebate == 0)),
                Arg.Any<CancellationToken>());
    }

    private Fixture CreateTurnoverWinLossFixture(Guid brandId, GameType gameType, decimal turnover)
    {
        var fixture = new Fixture();
        fixture.Customize<PlayerAllAgentTurnoverWinLossModel>(u => u
            .With(s => s.BrandId, brandId)
            .With(x => x.GameType, gameType)
            .With(x => x.Turnover, turnover));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }

    private static Fixture CreateValidBetConditionModelFixture(
        PlayerAllAgentTurnoverWinLossModel model)
    {
        var fixture = new Fixture();
        fixture.Customize<BrandAllAgentGameTypeConditionModel>(u
            => u.With(x => x.BrandId, model.BrandId)
                .With(x => x.AllAgentGameType, AllAgentGameType.Sportsbook)
                .With(x => x.AllAgentRebateType, AllAgentRebateType.ValidBet)
                .With(x => x.NetProfit, model.Turnover)
                .With(x => x.HasValidBet, true)
                .With(x => x.HasWinLoss, true)
                .With(x => x.ActiveNumber, 1));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}