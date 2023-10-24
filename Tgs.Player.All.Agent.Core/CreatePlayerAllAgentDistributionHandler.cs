using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Core.Workers;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class CreatePlayerAllAgentDistributionHandler : IRequestHandler<CreatePlayerAllAgentDistributionRequest>
{
    private readonly IPlayerAllAgentRepository _playerAllAgentRepository;
    private readonly IPlayerAllAgentRebateTaskQueue _taskQueue;
    private readonly ILogger<CreatePlayerAllAgentDistributionHandler> _logger;

    public CreatePlayerAllAgentDistributionHandler(IPlayerAllAgentRepository playerAllAgentRepository,
        IPlayerAllAgentRebateTaskQueue taskQueue,
        ILogger<CreatePlayerAllAgentDistributionHandler> logger)
    {
        _playerAllAgentRepository = playerAllAgentRepository;
        _taskQueue = taskQueue;
        _logger = logger;
    }

    public async Task Handle(CreatePlayerAllAgentDistributionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Player All Agent SettlementIntervalType:{SettlementIntervalType} From:{From} To:{To}",
            request.SettlementIntervalType,
            request.From,
            request.To);
        
        var validPlayerCodes =
            _playerAllAgentRepository.GetValidPlayersAsync(request.From,
                request.To,
                request.SettlementIntervalType);

        var brandIds = await validPlayerCodes.Select(x => x.BrandId).Distinct().ToArrayAsync(cancellationToken);

        var brandAllAgentGameTypeConditions =
            await _playerAllAgentRepository.GetBrandAllAgentGameTypeConditionsAsync(brandIds)
                .ToArrayAsync(cancellationToken);

        await validPlayerCodes.ForEachAwaitWithCancellationAsync(async (x, c) =>
            {
                var conditions = brandAllAgentGameTypeConditions.Where(b => b.BrandId == x.BrandId);

                if (conditions.Any())
                    await _taskQueue.QueuePlayerRebateAsync(new PlayerAllAgentRebateQueueModel
                    {
                        From = request.From,
                        To = request.To,
                        ParentPlayerCode = x.ParentPlayerCode,
                        Conditions = conditions
                    });
            },
            cancellationToken);
    }
}