using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class UpdateBrandAllAgentSwitchHandler : IRequestHandler<UpdateBrandAllAgentSwitchRequest>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<UpdateBrandAllAgentSwitchHandler> _logger;

    public UpdateBrandAllAgentSwitchHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<UpdateBrandAllAgentSwitchHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public Task Handle(UpdateBrandAllAgentSwitchRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand switch:{request}", request);

        return _brandAllAgentRepository.UpdateSwitchAsync(request.BrandId, request.Switch, cancellationToken);
    }
}