using MediatR;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class
    GetBrandAllAgentValidBetConditionsHandler : IStreamRequestHandler<GetBrandAllAgentValidBetConditionsRequest,
        ValidBetConditionViewModel>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;

    public GetBrandAllAgentValidBetConditionsHandler(
        IBrandAllAgentRepository brandAllAgentRepository)
    {
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public IAsyncEnumerable<ValidBetConditionViewModel> Handle(GetBrandAllAgentValidBetConditionsRequest request,
        CancellationToken cancellationToken)
    {
        return _brandAllAgentRepository.GetBrandAllAgentValidBetConditionsAsync(request.BrandId,
            request.AllAgentGameType);
    }
}