using AutoFixture;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Player.All.Agent.Core;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.UnitTest
{
    public class PlayerAllAgentManualDistributionTests
    {
        [Test]
        public async Task Create_Player_All_Agent_Distributions()
        {
            var fakePlayerAllAgentRepository = Substitute.For<IPlayerAllAgentRepository>();
            var fakeMediator = Substitute.For<IMediator>();

            var sut = new PlayerAllAgentManualDistributionHandler(
                fakePlayerAllAgentRepository,
                fakeMediator,
                NullLogger<PlayerAllAgentManualDistributionHandler>.Instance);

            var fixture = new Fixture();
            var contributors = fixture.CreateMany<PlayerAllAgentRebateContributorModel>(10);

            fakePlayerAllAgentRepository
                .GetNotSentRebatesAsync(Arg.Any<DateTime>(), Arg.Any<DateTime>())
                .Returns(contributors.ToAsyncEnumerable());

            var request = new PlayerAllAgentManualDistributionRequest(DateTime.UtcNow,DateTime.UtcNow);

            await sut.Handle(request, default);

            await fakeMediator.Received(10).Publish(Arg.Any<CreatePlayerBucketEvent>());
            await fakePlayerAllAgentRepository.Received(10).SetDistributedAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
            await fakeMediator.Received(10).Publish(Arg.Any<DistributeRebateEvent>());
        }
    }
}