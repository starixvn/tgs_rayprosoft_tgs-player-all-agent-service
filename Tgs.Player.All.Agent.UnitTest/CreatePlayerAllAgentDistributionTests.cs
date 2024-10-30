using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Player.All.Agent.Core;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.BackgroundQueue;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.UnitTest;

public class CreatePlayerAllAgentDistributionTests
{
    [Test]
    public async Task Create_Player_All_Agent_Distributions()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new CreatePlayerAllAgentDistributionHandler(
            fakePlayerAllAgentRepository,
            fakePlayerAllAgentRebateTaskQueue,
            NullLogger<CreatePlayerAllAgentDistributionHandler>.Instance);

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionViewModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        var request =
            new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Daily, DateTime.UtcNow, DateTime.UtcNow);

        await sut.Handle(request, default);

        await fakePlayerAllAgentRebateTaskQueue.Received(10)
            .QueuePlayerRebateAsync(Arg.Any<PlayerAllAgentRebateQueueModel>());
    }

    [Test]
    public async Task Create_Player_All_Agent_Distributions_When_Only_One_Brand_Has_Conditions()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakePlayerAllAgentRebateTaskQueue = Substitute.For<IPlayerAllAgentRebateTaskQueue>();

        var sut = new CreatePlayerAllAgentDistributionHandler(
            fakePlayerAllAgentRepository,
            fakePlayerAllAgentRebateTaskQueue,
            NullLogger<CreatePlayerAllAgentDistributionHandler>.Instance);

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var condition = CreateConditionFixture(validPlayers.First().BrandId)
            .Create<BrandAllAgentGameTypeConditionViewModel>();

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(new[] { condition }.ToAsyncEnumerable());

        var request =
            new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Daily, DateTime.UtcNow, DateTime.UtcNow);

        await sut.Handle(request, default);

        await fakePlayerAllAgentRebateTaskQueue.Received(1)
            .QueuePlayerRebateAsync(Arg.Any<PlayerAllAgentRebateQueueModel>());
    }

    private static Fixture CreateConditionFixture(Guid brandId)
    {
        var fixture = new Fixture();
        fixture.Customize<BrandAllAgentGameTypeConditionViewModel>(u => u
            .With(s => s.BrandId, brandId));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}