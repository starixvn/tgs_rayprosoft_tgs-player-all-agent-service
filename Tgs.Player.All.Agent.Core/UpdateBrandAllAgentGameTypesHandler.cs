using MediatR;
using Microsoft.Extensions.Logging;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core;

internal class UpdateBrandAllAgentGameTypesHandler : IRequestHandler<UpdateBrandAllAgentGameTypeSwitchRequest>
{
    private readonly IBrandAllAgentRepository _brandAllAgentRepository;
    private readonly ILogger<UpdateBrandAllAgentGameTypesHandler> _logger;

    public UpdateBrandAllAgentGameTypesHandler(
        IBrandAllAgentRepository brandAllAgentRepository,
        ILogger<UpdateBrandAllAgentGameTypesHandler> logger)
    {
        _logger = logger;
        _brandAllAgentRepository = brandAllAgentRepository;
    }

    public async Task Handle(UpdateBrandAllAgentGameTypeSwitchRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update brand game types:{request}",
            request);

        if (await _brandAllAgentRepository.CheckGameTypeExists(request.BrandId,
                    request.AllAgentGameType,
                    cancellationToken)
                .ConfigureAwait(false))
            await _brandAllAgentRepository.UpdateGameTypesAsync(request.BrandId,
                    request.Switch,
                    request.HasValidBet,
                    request.AllAgentGameType,
                    cancellationToken)
                .ConfigureAwait(false);
        else
            await _brandAllAgentRepository.CreateGameTypesAsync(request.BrandId,
                    request.Switch,
                    request.HasValidBet,
                    request.AllAgentGameType,
                    cancellationToken)
                .ConfigureAwait(false);
    }
}