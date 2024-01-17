using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.BackgroundQueue;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class PlayerAllAgentManualDistributionHandler : IRequestHandler<PlayerAllAgentManualDistributionRequest>
{
    private readonly ILogger<PlayerAllAgentManualDistributionHandler> _logger;
    private readonly IPlayerAllAgentRepository _playerAllAgentRepository;
    private readonly IPlayerAllAgentRebateDistributionTaskQueue _taskQueue;

    public PlayerAllAgentManualDistributionHandler(IPlayerAllAgentRepository playerAllAgentRepository,
        IPlayerAllAgentRebateDistributionTaskQueue taskQueue,
        ILogger<PlayerAllAgentManualDistributionHandler> logger)
    {
        _playerAllAgentRepository = playerAllAgentRepository;
        _taskQueue = taskQueue;
        _logger = logger;
    }

    public async Task Handle(PlayerAllAgentManualDistributionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player All Agent Manual Distribution From:{From} To:{To}",
            request.From,
            request.To);

        var readToSendRebates =
            await _playerAllAgentRepository.GetNotSentRebatesAsync(request.From, request.To)
                .ToArrayAsync(cancellationToken);

        foreach (var playerAllAgentRebateContributorModel in readToSendRebates)
        {
            await _taskQueue.QueuePlayerRebateDistributionAsync(new PlayerAllAgentRebateDistributionQueueModel
                { Contributor = playerAllAgentRebateContributorModel });
        }
    }
}