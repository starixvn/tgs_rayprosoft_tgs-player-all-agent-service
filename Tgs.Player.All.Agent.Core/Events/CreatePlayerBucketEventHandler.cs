using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Service.PlayerBuckets;

namespace Tgs.Player.All.Agent.Core.Events;

internal class CreatePlayerBucketEventHandler : INotificationHandler<CreatePlayerBucketEvent>
{
    private readonly ILogger<CreatePlayerBucketEventHandler> _logger;
    private readonly IPlayerBucketClient _playerBucketClient;

    public CreatePlayerBucketEventHandler(
        IPlayerBucketClient playerBucketClient,
    ILogger<CreatePlayerBucketEventHandler> logger)
    {
        _logger = logger;
        _playerBucketClient = playerBucketClient;
    }

    public async Task Handle(CreatePlayerBucketEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player All Agent ParentPlayerCode:{rebate.PlayerCode} RebateAmount:{totalRebate}",
            @event.PlayerAllAgentRebateContributor.PlayerCode,
            @event.PlayerAllAgentRebateContributor.TotalRebate);

        await Policy.Handle<Exception>()
            .RetryAsync(
                3,
                (exception, retryCount) => _logger.LogError(
                    "Player All Agent ParentPlayerCode:{rebate.PlayerCode} RebateAmount:{totalRebate} retryCount:{retryCount} Exception:{exception}",
                    @event.PlayerAllAgentRebateContributor.PlayerCode,
                    @event.PlayerAllAgentRebateContributor.TotalRebate,
                    retryCount,
                    exception))
            .ExecuteAsync(
                async () =>
                {
                    _ = await _playerBucketClient.CreateBucketAsync(
                            @event.PlayerAllAgentRebateContributor.PlayerCode,
                            @event.PlayerAllAgentRebateContributor.TotalRebate,
                            BucketType.AllAgent,
                            @event.PlayerAllAgentRebateContributor.Id.ToString(),
                            "",
                            cancellationToken)
                        .ConfigureAwait(false);
                });
    }
}