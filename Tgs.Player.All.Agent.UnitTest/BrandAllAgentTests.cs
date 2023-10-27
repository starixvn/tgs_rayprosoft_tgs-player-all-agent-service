using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Player.All.Agent.Core;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.UnitTest;

public class BrandAllAgentTests
{
    [Test]
    public async Task Get_Brand_All_Agent()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new GetBrandAllAgentSwitchHandler(
            fakeBrandAllAgentRepository,
            NullLogger<GetBrandAllAgentSwitchHandler>.Instance);

        var brandId = Guid.NewGuid();

        fakeBrandAllAgentRepository
            .GetBrandAllAgentAsync(Arg.Is(brandId), Arg.Any<CancellationToken>())
            .Returns(new BrandAllAgentViewModel(true, null));

        var request = new GetBrandAllAgentRequest(brandId);

        var response = await sut.Handle(request, default);

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Switch, Is.True);
            Assert.That(response.SettlementIntervalType, Is.Null);
        });
    }

    [Test]
    public async Task Get_Brand_All_Agent_Game_Types()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new GetBrandAllAgentGameTypesHandler(
            fakeBrandAllAgentRepository,
            NullLogger<GetBrandAllAgentGameTypesHandler>.Instance);

        var brandId = Guid.NewGuid();

        fakeBrandAllAgentRepository
            .GetBrandAllAgentGameTypesAsync(Arg.Is(brandId))
            .Returns(new[]
            {
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = true,
                    AllAgentGameType = AllAgentGameType.CardGame
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = true,
                    AllAgentGameType = AllAgentGameType.Slot
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = true,
                    AllAgentGameType = AllAgentGameType.LiveDealer
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = true,
                    AllAgentGameType = AllAgentGameType.Sportsbook
                }
            }.ToAsyncEnumerable());

        var request = new GetBrandAllAgentGameTypesRequest(brandId);

        var response = await sut.Handle(request, default).ToArrayAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any(x => x is { AllAgentGameType : AllAgentGameType.CardGame, Switch: true, HasValidBet: true}));
            Assert.That(response.Any(x => x is { AllAgentGameType: AllAgentGameType.Slot, Switch: true, HasValidBet: true }));
            Assert.That(response.Any(x => x is { AllAgentGameType: AllAgentGameType.LiveDealer, Switch: true, HasValidBet: true }));
            Assert.That(response.Any(x => x is { AllAgentGameType: AllAgentGameType.Sportsbook, Switch: true, HasValidBet: true }));
            Assert.That(response.Any(x => x is { AllAgentGameType: AllAgentGameType.VirtualSports, Switch: false, HasValidBet: false }));
            Assert.That(response.Any(x => x is { AllAgentGameType: AllAgentGameType.Lottery, Switch: false, HasValidBet: false }));
        });
    }

    [Test]
    public async Task Get_Brand_All_Agent_Valid_Bet_Conditions()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new GetBrandAllAgentValidBetConditionsHandler(
            fakeBrandAllAgentRepository,
            NullLogger<GetBrandAllAgentValidBetConditionsHandler>.Instance);

        var brandId = Guid.NewGuid();

        fakeBrandAllAgentRepository
            .GetBrandAllAgentValidBetConditionsAsync(Arg.Is(brandId), Arg.Is<AllAgentGameType>(AllAgentGameType.CardGame))
            .Returns(new[]
            {
                new ValidBetConditionViewModel
                {
                    Rank = 1,
                    ActiveNumber = 1,
                    NetProfit = 100,
                    RebatePercentage = 10
                },
                new ValidBetConditionViewModel
                {
                    Rank = 2,
                    ActiveNumber = 2,
                    NetProfit = 100,
                    RebatePercentage = 10
                }
            }.ToAsyncEnumerable());

        var request = new GetBrandAllAgentValidBetConditionsRequest(brandId, AllAgentGameType.CardGame);

        var response = await sut.Handle(request, default).ToArrayAsync();

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Any(x => x is { Rank: 1, ActiveNumber: 1, NetProfit: 100, RebatePercentage: 10 }));
            Assert.That(response.Any(x => x is { Rank: 2, ActiveNumber: 2, NetProfit: 100, RebatePercentage: 10 }));
        });
    }

    [Test]
    public async Task Update_Brand_All_Agent_Switch()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new UpdateBrandAllAgentSwitchHandler(
            fakeBrandAllAgentRepository,
            NullLogger<UpdateBrandAllAgentSwitchHandler>.Instance);

        var brandId = Guid.NewGuid();

        var request = new UpdateBrandAllAgentSwitchRequest(brandId, true);

        await sut.Handle(request, default);

        _ = fakeBrandAllAgentRepository.Received(1)
            .UpdateSwitchAsync(Arg.Is(brandId), Arg.Is(true), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Update_Brand_All_Agent_Settlement_Interval()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new UpdateBrandAllAgentSettleIntervalHandler(
            fakeBrandAllAgentRepository,
            NullLogger<UpdateBrandAllAgentSettleIntervalHandler>.Instance);

        var brandId = Guid.NewGuid();

        var request = new UpdateBrandAllAgentSettlementIntervalRequest(brandId, SettlementIntervalType.Weekly);

        await sut.Handle(request, default);

        _ = fakeBrandAllAgentRepository.Received(1)
            .UpdateSettleIntervalAsync(Arg.Is(brandId), Arg.Is(SettlementIntervalType.Weekly), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Update_Brand_All_Agent_Game_Type_Switch_When_Is_Exists()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new UpdateBrandAllAgentGameTypesHandler(
            fakeBrandAllAgentRepository,
            NullLogger<UpdateBrandAllAgentGameTypesHandler>.Instance);

        var brandId = Guid.NewGuid();

        fakeBrandAllAgentRepository
            .CheckGameTypeExists(Arg.Is(brandId), Arg.Is(AllAgentGameType.CardGame), Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new UpdateBrandAllAgentGameTypeSwitchRequest(brandId, true, true, AllAgentGameType.CardGame);

        await sut.Handle(request, default);

        _ = fakeBrandAllAgentRepository.Received(1)
            .UpdateGameTypesAsync(Arg.Is(brandId), Arg.Is(true), Arg.Is(true), Arg.Is(AllAgentGameType.CardGame), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Update_Brand_All_Agent_Game_Type_Switch_When_Is_Not_Exists()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new UpdateBrandAllAgentGameTypesHandler(
            fakeBrandAllAgentRepository,
            NullLogger<UpdateBrandAllAgentGameTypesHandler>.Instance);

        var brandId = Guid.NewGuid();

        fakeBrandAllAgentRepository
            .CheckGameTypeExists(Arg.Is(brandId), Arg.Is(AllAgentGameType.CardGame), Arg.Any<CancellationToken>())
            .Returns(false);

        var request = new UpdateBrandAllAgentGameTypeSwitchRequest(brandId, true, true, AllAgentGameType.CardGame);

        await sut.Handle(request, default);

        _ = fakeBrandAllAgentRepository.Received(1)
            .CreateGameTypesAsync(Arg.Is(brandId), Arg.Is(true), Arg.Is(true), Arg.Is(AllAgentGameType.CardGame), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Update_Brand_All_Agent_Valid_Bet_Conditions()
    {
        var fakeBrandAllAgentRepository = Substitute.For<IBrandAllAgentRepository>();

        var sut = new UpdateBrandAllAgentValidBetConditionHandler(
            fakeBrandAllAgentRepository,
            NullLogger<UpdateBrandAllAgentValidBetConditionHandler>.Instance);

        var brandId = Guid.NewGuid();

        var conditions = new[]
        {
            new ValidBetConditionViewModel
            {
                Rank = 1,
                ActiveNumber = 1,
                NetProfit = 100,
                RebatePercentage = 10
            },
            new ValidBetConditionViewModel
            {
                Rank = 2,
                ActiveNumber = 2,
                NetProfit = 100,
                RebatePercentage = 10
            }
        };

        var request = new UpdateBrandAllAgentValidBetConditionRequest(brandId, conditions, AllAgentGameType.LiveDealer);

        await sut.Handle(request, default);

        _ = fakeBrandAllAgentRepository.Received(1)
            .UpdateValidBetConditionAsync(Arg.Is(brandId), Arg.Is(AllAgentGameType.LiveDealer), Arg.Is(conditions), Arg.Any<CancellationToken>());
    }
}