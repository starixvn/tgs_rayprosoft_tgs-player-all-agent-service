using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using Tgs.Player.All.Agent.Client;
using Tgs.Player.All.Agent.Client.Models;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;
using Tgs.Player.All.Agent.Repository.Entities;
using AllAgentGameType = Tgs.Player.All.Agent.Domain.Enums.AllAgentGameType;
using GetBrandAllAgentRequest = Tgs.Player.All.Agent.Domain.Models.GetBrandAllAgentRequest;
using SettlementIntervalType = Tgs.Player.All.Agent.Domain.Enums.SettlementIntervalType;

namespace Tgs.Player.All.Agent.IntegrationTest;

[TestFixture]
[Parallelizable]
public class BrandAllAgentTests
{
    [Test]
    public async Task Get_Brand_All_Agent()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        fakeMediator.Send(Arg.Any<GetBrandAllAgentRequest>())
            .Returns(new BrandAllAgentViewModel(true, SettlementIntervalType.Monthly));

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new Client.Models.GetBrandAllAgentRequest(brandId);

            var response = await sut.GetBrandAllAgentAsync(request);

            Assert.That(response, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(response.Switch, Is.EqualTo(true));
                Assert.That(response.SettlementIntervalType, Is.EqualTo(Client.SettlementIntervalType.Monthly));
            });

            await fakeMediator.Received(1).Send(Arg.Any<GetBrandAllAgentRequest>());
        }
    }

    [Test]
    public async Task Get_Brand_All_Agent_When_SettleIntervalType_Is_Null()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        fakeMediator.Send(Arg.Any<GetBrandAllAgentRequest>())
            .Returns(new BrandAllAgentViewModel(true, null));

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new Client.Models.GetBrandAllAgentRequest(brandId);

            var response = await sut.GetBrandAllAgentAsync(request);

            Assert.That(response, Is.Not.Null);

            Assert.Multiple(() =>
            {
                Assert.That(response.Switch, Is.EqualTo(true));
                Assert.That(response.SettlementIntervalType, Is.EqualTo(null));
            });

            await fakeMediator.Received(1).Send(Arg.Any<GetBrandAllAgentRequest>());
        }
    }

    [Test]
    public async Task Get_Brand_All_Agent_Game_Types()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        fakeMediator.CreateStream(Arg.Any<GetBrandAllAgentGameTypesRequest>())
            .Returns(new BrandAllAgentGameTypeViewModel[]
            {
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = true,
                    AllAgentGameType = AllAgentGameType.Sportsbook
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = true,
                    HasValidBet = false,
                    AllAgentGameType = AllAgentGameType.VirtualSports
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
                    HasValidBet = false,
                    AllAgentGameType = AllAgentGameType.Slot
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = false,
                    HasValidBet = false,
                    AllAgentGameType = AllAgentGameType.Lottery
                },
                new BrandAllAgentGameTypeViewModel
                {
                    Switch = false,
                    HasValidBet = false,
                    AllAgentGameType = AllAgentGameType.CardGame
                }
            }.ToAsyncEnumerable());

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new GetGameTypesRequest(brandId);

            var response = await sut.GetGameTypesAsync(request).ToArrayAsync();

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Has.Length.EqualTo(6));
            Assert.That(response, Does.Contain(new GameTypeModel
            {
                Switch = false,
                HasValidBet = false,
                AllAgentGameType = Client.AllAgentGameType.Lottery
            }));
            Assert.That(response, Does.Contain(new GameTypeModel
            {
                Switch = false,
                HasValidBet = false,
                AllAgentGameType = Client.AllAgentGameType.CardGame
            }));
            Assert.That(response, Does.Contain(new GameTypeModel
            {
                Switch = true,
                HasValidBet = true,
                AllAgentGameType = Client.AllAgentGameType.Sportsbook
            }));

            _ = fakeMediator.Received(1).CreateStream(Arg.Any<GetBrandAllAgentGameTypesRequest>());
        }
    }

    [Test]
    public async Task Get_Brand_All_Agent_Valid_Bet_Conditions()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        fakeMediator.CreateStream(Arg.Any<GetBrandAllAgentValidBetConditionsRequest>())
            .Returns(new ValidBetConditionViewModel[]
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

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
            {
                service.AddSingleton(fakeMediator);
            }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new GetValidBetConditionsRequest(brandId, Client.AllAgentGameType.LiveDealer);

            var response = await sut.GetValidBetConditionsAsync(request).ToArrayAsync();

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Has.Length.EqualTo(2));
            Assert.Multiple(() =>
            {
                Assert.That(response.Any(x => x is { Rank: 1, ActiveNumber: 1, NetProfit: 100, RebatePercentage: 10 }));
                Assert.That(response.Any(x => x is { Rank: 2, ActiveNumber: 2, NetProfit: 100, RebatePercentage: 10 }));
            });

            _ = fakeMediator.Received(1).CreateStream(Arg.Any<GetBrandAllAgentValidBetConditionsRequest>());
        }
    }

    [Test]
    public async Task Update_Brand_All_Agent_Switch()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
            {
                service.AddSingleton(fakeMediator);
            }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new UpdateSwitchRequest(brandId, true);

            await sut.UpdateSwitchAsync(request);

            _ = fakeMediator.Received(1).Send(Arg.Is<UpdateBrandAllAgentSwitchRequest>(new UpdateBrandAllAgentSwitchRequest(brandId, true)));
        }
    }

    [Test]
    public async Task Update_Brand_All_Agent_Settlement_Interval()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new UpdateSettlementIntervalRequest(brandId, Client.SettlementIntervalType.Monthly);

            await sut.UpdateSettlementIntervalAsync(request);

            _ = fakeMediator.Received(1).Send(Arg.Is<UpdateBrandAllAgentSettlementIntervalRequest>(new UpdateBrandAllAgentSettlementIntervalRequest(brandId, SettlementIntervalType.Monthly)));
        }
    }

    [Test]
    public async Task Update_Brand_All_Agent_Game_Type_Switch()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new UpdateGameTypeSwitchRequest(brandId, true, true, Client.AllAgentGameType.CardGame);

            await sut.UpdateGameTypeSwitchAsync(request);

            _ = fakeMediator.Received(1).Send(Arg.Is<UpdateBrandAllAgentGameTypeSwitchRequest>(new UpdateBrandAllAgentGameTypeSwitchRequest(brandId, true, true, AllAgentGameType.CardGame)));
        }
    }

    [Test]
    public async Task Update_Brand_All_Agent_Valid_Bet_Conditions()
    {
        var fakeMediator = Substitute.For<IMediator>();

        var brandId = Guid.NewGuid();

        await using var web = new TestApplication(builder =>
            builder.ConfigureServices(service =>
                {
                    service.AddSingleton(fakeMediator);
                }
            ));

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new UpdateGameTypeSwitchRequest(brandId, true, true, Client.AllAgentGameType.CardGame);

            await sut.UpdateGameTypeSwitchAsync(request);

            _ = fakeMediator.Received(1).Send(Arg.Is<UpdateBrandAllAgentGameTypeSwitchRequest>(new UpdateBrandAllAgentGameTypeSwitchRequest(brandId, true, true, AllAgentGameType.CardGame)));
        }
    }

    [Test]
    public async Task Get_Brand_All_All_Agent_Valid_Bet_Conditions()
    {
        var brandId = Guid.NewGuid();

        await using var web = new TestApplication();

        var dbContext = web.Services.GetRequiredService<tgsContext>();

        var condition = new BrandAllAgentGameTypeCondition
        {
            AllAgentGameType = (int)AllAgentGameType.CardGame,
            AllAgentRebateType = (short)AllAgentRebateType.ValidBet,
            BrandId = brandId,
            Rank = 1,
        };

        _ = await dbContext.BrandAllAgentGameTypeConditions.AddAsync(condition);

        _ = await dbContext.SaveChangesAsync();

        var httpClient = web.CreateClient();
        {
            var sut = new BrandAllAgentClient(httpClient);

            var request = new GetAllValidBetConditionsRequest(brandId);

            var response = await sut.GetAllValidBetConditionsAsync(request).ToArrayAsync();

            Assert.That(response, Is.Not.Null);
            Assert.That(response, Has.Length.EqualTo(1));
            Assert.Multiple(() =>
            {
                Assert.That(response.Any(x => x is { Rank: 1, ActiveNumber: 0, NetProfit: 0, RebatePercentage: 0 }));
            });
        }
    }
}