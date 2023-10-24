using MediatR;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.CashService.Client;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.PlayerService.Client;

namespace Tgs.Player.All.Agent.Core.Events;

internal class DistributeRebateEventHandler : INotificationHandler<DistributeRebateEvent>
{
    private readonly ICashServiceClient _cashServiceClient;
    private readonly IPlayerAllAgentRepository _playerRepository;
    private readonly IPlayerServiceClient _playerServiceClient;
    private readonly ILogger<CreatePlayerBucketEventHandler> _logger;

    public DistributeRebateEventHandler(
        ICashServiceClient cashServiceClient,
        IPlayerAllAgentRepository playerRepository,
        IPlayerServiceClient playerServiceClient,
        ILogger<CreatePlayerBucketEventHandler> logger)
    {
        _logger = logger;
        _cashServiceClient = cashServiceClient;
        _playerServiceClient = playerServiceClient;
        _playerRepository = playerRepository;
    }

    public async Task Handle(DistributeRebateEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player All Agent ParentPlayerCode:{PlayerCode} RebateAmount:{totalRebate}",
            @event.PlayerAllAgentRebateContributor.PlayerCode,
            @event.PlayerAllAgentRebateContributor.TotalRebate);

        var parentPlayer = await _playerServiceClient
            .GetPlayerPropertiesAsync(@event.PlayerAllAgentRebateContributor.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        var defaultWalletId = Guid.Parse(parentPlayer["DefaultWalletId"]);

        var player = await _playerServiceClient
            .GetPlayerInfoAsync(@event.PlayerAllAgentRebateContributor.ChildPlayerCode, cancellationToken)
            .ConfigureAwait(false);

        await Policy.Handle<HttpRequestException>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(10),
                (exception, time, retryCount, context) => _logger.LogError(
                    "Player All Agent externalId:{id} ParentPlayerCode:{PlayerCode} RebateAmount:{totalRebate} retryCount:{retryCount} Exception:{exception}",
                    @event.PlayerAllAgentRebateContributor.Id,
                    @event.PlayerAllAgentRebateContributor.PlayerCode,
                    @event.PlayerAllAgentRebateContributor.TotalRebate,
                    retryCount,
                    exception))
            .ExecuteAsync(
                async () =>
                {
                    if (!await _playerRepository.CheckTransactionByExternalIdExistsAsync(
                            @event.PlayerAllAgentRebateContributor.PlayerCode,
                            @event.PlayerAllAgentRebateContributor.Id.ToString(),
                            cancellationToken))
                        _ = await _cashServiceClient.DividendAllAgentRebateAsync(
                                defaultWalletId,
                                @event.PlayerAllAgentRebateContributor.TotalRebate,
                                $"player name: {player?.UserName}",
                                @event.PlayerAllAgentRebateContributor.Id.ToString(),
                                cancellationToken)
                            .ConfigureAwait(false);
                });
    }
}