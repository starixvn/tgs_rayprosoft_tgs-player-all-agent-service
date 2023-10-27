using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class
    GetBrandAllAgentValidBetConditionsHandler : IStreamRequestHandler<GetBrandAllAgentValidBetConditionsRequest,
        ValidBetConditionViewModel>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<GetBrandAllAgentValidBetConditionsHandler> _logger;

    public GetBrandAllAgentValidBetConditionsHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<GetBrandAllAgentValidBetConditionsHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public IAsyncEnumerable<ValidBetConditionViewModel> Handle(GetBrandAllAgentValidBetConditionsRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand switch:{request}", request);

        return _brandAllAgentRepository.GetBrandAllAgentValidBetConditionsAsync(request.BrandId,
            request.AllAgentGameType);
    }
}