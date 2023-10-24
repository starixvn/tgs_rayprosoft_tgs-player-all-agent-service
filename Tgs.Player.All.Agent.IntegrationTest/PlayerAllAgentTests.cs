using AutoFixture;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.Player.All.Agent.Client;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.IntegrationTest;

[TestFixture]
[Parallelizable]
public class PlayerAllAgentTests
{
    [Test]
    public async Task Create_Daily_Player_All_Agent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            await sut.CreateDailyPlayerAllAgentRebatesAsync();

            await fakeMediator.Received(1).Send(Arg.Any<CreatePlayerAllAgentDistributionRequest>());
        }
    }

    [Test]
    public async Task Create_Weekly_Player_All_Agent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            await sut.CreateWeeklyPlayerAllAgentRebatesAsync();

            await fakeMediator.Received(1).Send(Arg.Any<CreatePlayerAllAgentDistributionRequest>());
        }
    }

    [Test]
    public async Task Create_Monthly_Player_All_Agent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            await sut.CreateMonthlyPlayerAllAgentRebatesAsync();

            await fakeMediator.Received(1).Send(Arg.Any<CreatePlayerAllAgentDistributionRequest>());
        }
    }

    [Test]
    public async Task Manual_Create_Daily_Player_All_Agent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            await sut.ManualCreateDailyPlayerAllAgentRebatesAsync(DateOnly.Parse("2023-01-01"));

            await fakeMediator.Received(1).Send(Arg.Any<CreatePlayerAllAgentDistributionRequest>());
        }
    }

    [Test]
    public async Task Manual_Create_Weekly_Player_All_Agent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            await sut.ManualCreateWeeklyPlayerAllAgentRebatesAsync(DateOnly.Parse("2023-01-01"));

            await fakeMediator.Received(1).Send(Arg.Any<CreatePlayerAllAgentDistributionRequest>());
        }
    }

    [Test]
    public async Task Manual_Distribute_Not_Sent_Rebates()
    {
        var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
        var fakeMediator = Substitute.For<IMediator>();

        var fixture = new Fixture();
        var validPlayers = fixture.CreateMany<ValidPlayer>(10);

        var conditions = validPlayers.Select(x => x.BrandId)
            .Select(x => CreateConditionFixture(x).Create<BrandAllAgentGameTypeConditionModel>());

        fakePlayerAllAgentRepository
            .GetValidPlayersAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<SettlementIntervalType>())
            .Returns(validPlayers.ToAsyncEnumerable());

        fakePlayerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(Arg.Any<Guid[]>())
            .Returns(conditions.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakePlayerAllAgentRepository);
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new PlayerAllAgentClient(httpClient);

            var from = DateTimeOffset.Parse("2023-10-16 00:00+8");
            var to = DateTimeOffset.Parse("2023-10-23 00:00+8");
            await sut.DistributeRebatesAsync(DateOnly.Parse("2023-10-16"), DateOnly.Parse("2023-10-23"));

            await fakeMediator.Received(1)
                .Send(Arg.Is(new PlayerAllAgentManualDistributionRequest(from.UtcDateTime, to.UtcDateTime)));
        }
    }

    private Fixture CreateConditionFixture(Guid brandId)
    {
        var fixture = new Fixture();
        fixture.Customize<BrandAllAgentGameTypeConditionModel>(u => u
            .With(s => s.BrandId, brandId));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        return fixture;
    }
}