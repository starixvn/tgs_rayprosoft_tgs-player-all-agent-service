using MediatR;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class
    GetBrandAllAllAgentValidBetConditionsHandler : IStreamRequestHandler<GetBrandAllAllAgentValidBetConditionsRequest,
        ValidBetConditionViewModel>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;

    public GetBrandAllAllAgentValidBetConditionsHandler(
        IBrandAllAgentRepository brandAllAgentRepository)
    {
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public IAsyncEnumerable<ValidBetConditionViewModel> Handle(GetBrandAllAllAgentValidBetConditionsRequest request,
        CancellationToken cancellationToken)
        => _brandAllAgentRepository.GetBrandAllAllAgentValidBetConditionsAsync(request.BrandId);
}