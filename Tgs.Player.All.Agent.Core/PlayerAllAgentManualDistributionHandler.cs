using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Events;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class PlayerAllAgentManualDistributionHandler : IRequestHandler<PlayerAllAgentManualDistributionRequest>
{
    private readonly IPlayerAllAgentRepository _playerAllAgentRepository;
    private readonly IMediator _mediator;
    private readonly ILogger<PlayerAllAgentManualDistributionHandler> _logger;

    public PlayerAllAgentManualDistributionHandler(IPlayerAllAgentRepository playerAllAgentRepository,
        IMediator mediator,
        ILogger<PlayerAllAgentManualDistributionHandler> logger)
    {
        _playerAllAgentRepository = playerAllAgentRepository;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(PlayerAllAgentManualDistributionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player All Agent Manual Distribution From:{From} To:{To}",
            request.From,
            request.To);
        
        var readToSendRebates =
            _playerAllAgentRepository.GetNotSentRebatesAsync(request.From, request.To);

        await readToSendRebates.ForEachAwaitWithCancellationAsync(async (x, c) =>
            {
                await _mediator.Publish(new CreatePlayerBucketEvent(x), cancellationToken);

                await _playerAllAgentRepository.SetDistributedAsync(x.Id, cancellationToken);

                await _mediator.Publish(new DistributeRebateEvent(x), cancellationToken);
            },
            cancellationToken);
    }
}