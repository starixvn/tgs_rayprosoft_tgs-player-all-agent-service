using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;

namespace Tgs.Player.All.Agent.Core.Workers;

public interface IPlayerAllAgentRebateDistributionTaskQueueProcessor
{
    Task ProcessQueueAsync(CancellationToken cancellationToken);
}

public class PlayerAllAgentRebateDistributionTaskQueueProcessor : IPlayerAllAgentRebateDistributionTaskQueueProcessor
{
    private readonly ILogger<PlayerAllAgentRebateDistributionTaskQueueProcessor> _logger;
    private readonly IMediator _mediator;
    private readonly IPlayerAllAgentRepository _playerAllAgentRepository;
    private readonly IPlayerAllAgentRebateDistributionTaskQueue _taskQueue;

    public PlayerAllAgentRebateDistributionTaskQueueProcessor(
        IPlayerAllAgentRebateDistributionTaskQueue taskQueue,
        IPlayerAllAgentRepository playerAllAgentRepository,
        ILogger<PlayerAllAgentRebateDistributionTaskQueueProcessor> logger,
        IMediator mediator)
    {
        _taskQueue = taskQueue;
        _logger = logger;
        _mediator = mediator;
        _playerAllAgentRepository = playerAllAgentRepository;
    }

    public async Task ProcessQueueAsync(CancellationToken cancellationToken)
    {
        var request = await _taskQueue.DequeueAsync(cancellationToken);

        _logger.LogInformation("player all agent rebate distribution request: {request}", request);

        await _playerAllAgentRepository.SetDistributedAsync(request.Contributor.Id, cancellationToken);

        await _mediator.Publish(new CreatePlayerBucketEvent(request.Contributor), cancellationToken);

        await _mediator.Publish(new DistributeRebateEvent(request.Contributor), cancellationToken);

        await Task.Delay(200, cancellationToken);
    }
}