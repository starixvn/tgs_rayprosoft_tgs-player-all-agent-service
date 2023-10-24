using System.Collections.Immutable;
using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.CashService.Client;
using Tgs.Player.All.Agent.Core;
using Tgs.Player.All.Agent.Core.Events;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;
using Tgs.PlayerService.Client;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Player.All.Agent.UnitTest.Events
{
    public class DistributeRebateEventTests
    {
        [Test]
        public async Task Create_Distribute_Rebate_Event()
        {
            var fakeCashServiceClient = Substitute.For<ICashServiceClient>();
            var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
            var fakePlayerServiceClient = Substitute.For<IPlayerServiceClient>();

            var sut = new DistributeRebateEventHandler(
                fakeCashServiceClient,
                fakePlayerAllAgentRepository,
                fakePlayerServiceClient,
                NullLogger<CreatePlayerBucketEventHandler>.Instance);

            var fixture = new Fixture();
            var @event = fixture.Create<DistributeRebateEvent>();
            var walletId = Guid.NewGuid();

            fakePlayerServiceClient.GetPlayerPropertiesAsync(Arg.Any<string>())
                .Returns(new Dictionary<string, string>
                {
                    {"DefaultWalletId",walletId.ToString()}
                }.ToImmutableDictionary());

            var playerInfo = fixture.Create<PlayerInfo>();

            fakePlayerServiceClient.GetPlayerInfoAsync(Arg.Any<string>()).Returns(playerInfo);

            await sut.Handle(@event, default);

            await fakeCashServiceClient.Received(1)
                .DividendAllAgentRebateAsync(Arg.Is(walletId), Arg.Is(@event.PlayerAllAgentRebateContributor.TotalRebate), Arg.Any<string>(), Arg.Is(@event.PlayerAllAgentRebateContributor.Id.ToString()));
        }
    }
}