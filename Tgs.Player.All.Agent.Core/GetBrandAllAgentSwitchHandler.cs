using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class GetBrandAllAgentSwitchHandler : IRequestHandler<GetBrandAllAgentRequest, BrandAllAgentViewModel>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<GetBrandAllAgentSwitchHandler> _logger;

    public GetBrandAllAgentSwitchHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<GetBrandAllAgentSwitchHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public Task<BrandAllAgentViewModel> Handle(GetBrandAllAgentRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand switch:{request}", request);

        return _brandAllAgentRepository.GetBrandAllAgentAsync(request.BrandId, cancellationToken);
    }
}