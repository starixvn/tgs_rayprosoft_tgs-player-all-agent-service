using AutoFixture;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Player.All.Agent.Core;
using Tgs.Player.All.Agent.Core.Events;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Player.All.Agent.UnitTest.Events
{
    public class CreatePlayerBucketEventTests
    {
        [Test]
        public async Task Create_Player_Bucket_Event()
        {
            var fakePlayerBucketClient = Substitute.For<IPlayerBucketClient>();

            var sut = new CreatePlayerBucketEventHandler(
                fakePlayerBucketClient,
                NullLogger<CreatePlayerBucketEventHandler>.Instance);

            var fixture = new Fixture();
            var @event = fixture.Create<CreatePlayerBucketEvent>();

            await sut.Handle(@event, default);

            await fakePlayerBucketClient.Received(1)
                .CreateBucketAsync(Arg.Is(@event.PlayerAllAgentRebateContributor.PlayerCode), Arg.Is(@event.PlayerAllAgentRebateContributor.TotalRebate), Arg.Is(BucketType.AllAgent), Arg.Is(@event.PlayerAllAgentRebateContributor.Id.ToString()), Arg.Is(""));
        }
    }
}