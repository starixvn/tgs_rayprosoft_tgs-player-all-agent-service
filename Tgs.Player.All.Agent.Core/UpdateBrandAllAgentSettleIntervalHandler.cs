using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class UpdateBrandAllAgentSettleIntervalHandler : IRequestHandler<UpdateBrandAllAgentSettlementIntervalRequest>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<UpdateBrandAllAgentSettleIntervalHandler> _logger;

    public UpdateBrandAllAgentSettleIntervalHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<UpdateBrandAllAgentSettleIntervalHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public Task Handle(UpdateBrandAllAgentSettlementIntervalRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand settle interval:{request}", request);

        return _brandAllAgentRepository.UpdateSettleIntervalAsync(request.BrandId,
            request.SettlementIntervalType,
            cancellationToken);
    }
}