using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class
    UpdateBrandAllAgentValidBetConditionHandler : IRequestHandler<UpdateBrandAllAgentValidBetConditionRequest>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<UpdateBrandAllAgentValidBetConditionHandler> _logger;

    public UpdateBrandAllAgentValidBetConditionHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<UpdateBrandAllAgentValidBetConditionHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public Task Handle(UpdateBrandAllAgentValidBetConditionRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand all agent conditions:{request}",
            request);

        return _brandAllAgentRepository.UpdateValidBetConditionAsync(request.BrandId,
            request.AllAgentGameType,
            request.Conditions,
            cancellationToken);
    }
}