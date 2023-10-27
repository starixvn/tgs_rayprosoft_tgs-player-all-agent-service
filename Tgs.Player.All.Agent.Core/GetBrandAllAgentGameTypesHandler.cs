using System.Runtime.CompilerServices;
using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class
    GetBrandAllAgentGameTypesHandler : IStreamRequestHandler<GetBrandAllAgentGameTypesRequest,
        BrandAllAgentGameTypeViewModel>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<GetBrandAllAgentGameTypesHandler> _logger;

    public GetBrandAllAgentGameTypesHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<GetBrandAllAgentGameTypesHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public async IAsyncEnumerable<BrandAllAgentGameTypeViewModel> Handle(GetBrandAllAgentGameTypesRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand game types:{request}",
            request);

        var gameTypes = await _brandAllAgentRepository.GetBrandAllAgentGameTypesAsync(request.BrandId)
            .ToArrayAsync(cancellationToken);

        foreach (var gameType in Enum.GetValues(typeof(AllAgentGameType))
                     .Cast<AllAgentGameType>()
                     .GroupJoin(
                         gameTypes,
                         t => t,
                         g => g.AllAgentGameType,
                         (t, g) => new
                         {
                             AllAgentGameType = t,
                             BrandAllAgentGameTypes = g
                         })
                     .Select(x => new BrandAllAgentGameTypeViewModel
                     {
                         AllAgentGameType = x.AllAgentGameType,
                         Switch = x.BrandAllAgentGameTypes.FirstOrDefault()?.Switch ?? false,
                         HasValidBet = x.BrandAllAgentGameTypes.FirstOrDefault()?.HasValidBet ?? false
                     }))
            yield return gameType;
    }
}