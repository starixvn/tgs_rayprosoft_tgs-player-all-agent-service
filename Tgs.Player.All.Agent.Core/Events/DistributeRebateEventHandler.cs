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
    private readonly ILogger<CreatePlayerBucketEventHandler> _logger;
    private readonly IPlayerServiceClient _playerServiceClient;

    public DistributeRebateEventHandler(
        ICashServiceClient cashServiceClient,
        IPlayerServiceClient playerServiceClient,
        ILogger<CreatePlayerBucketEventHandler> logger)
    {
        _logger = logger;
        _cashServiceClient = cashServiceClient;
        _playerServiceClient = playerServiceClient;
    }

    public async Task Handle(DistributeRebateEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player all agent distribution ParentPlayerCode:{PlayerCode} ChildPlayerCode:{ChildPlayerCode} RebateAmount:{totalRebate}",
            @event.PlayerAllAgentRebateContributor.PlayerCode,
            @event.PlayerAllAgentRebateContributor.ChildPlayerCode,
            @event.PlayerAllAgentRebateContributor.TotalRebate);

        var parentPlayer = await _playerServiceClient
            .GetPlayerPropertiesAsync(@event.PlayerAllAgentRebateContributor.PlayerCode, cancellationToken)
            .ConfigureAwait(false);

        var defaultWalletId = Guid.Parse(parentPlayer["DefaultWalletId"]);

        var player = await _playerServiceClient
            .GetPlayerInfoAsync(@event.PlayerAllAgentRebateContributor.ChildPlayerCode, cancellationToken)
            .ConfigureAwait(false);

        _ = await _cashServiceClient.DividendAllAgentRebateAsync(
            defaultWalletId,
            @event.PlayerAllAgentRebateContributor.TotalRebate,
            $"player name: {player?.UserName}",
            @event.PlayerAllAgentRebateContributor.Id.ToString(),
            cancellationToken);
    }
}