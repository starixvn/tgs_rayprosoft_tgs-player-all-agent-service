using System.ComponentModel;
using System.Net;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BrandAllAgentController : ControllerBase
{
    private readonly IMediator _mediator;

    public BrandAllAgentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPatch("switch")]
    [Description("Update brand all agent switch")]
    public Task UpdateSwitchAsync(UpdateBrandAllAgentSwitchRequest request)
    {
        return _mediator.Send(request);
    }

    [HttpPut("game-type")]
    [Description("Update brand all agent game type")]
    public Task UpdateGameTypesAsync(UpdateBrandAllAgentGameTypeSwitchRequest request)
    {
        return _mediator.Send(request);
    }

    [HttpPut("settlement-interval")]
    [Description("Update brand all agent settle interval")]
    public Task UpdateBrandAllAgentSettlementIntervalAsync(UpdateBrandAllAgentSettlementIntervalRequest request)
    {
        return _mediator.Send(request);
    }

    [HttpPut("valid-bet-conditions")]
    [Description("Update brand all agent valid bet conditionss")]
    public Task UpdateValidBetConditionsAsync(UpdateBrandAllAgentValidBetConditionRequest request)
    {
        return _mediator.Send(request);
    }

    [HttpGet("{brandId}/switch")]
    [Description("Get brand all agent switch")]
    public Task<BrandAllAgentViewModel> UpdateGameTypeConditionsAsync(Guid brandId)
    {
        return _mediator.Send(new GetBrandAllAgentRequest(brandId));
    }

    [HttpGet("{brandId}/game-types")]
    [Description("Get brand all agent game types")]
    public IAsyncEnumerable<BrandAllAgentGameTypeViewModel> GetGameTypesAsync(Guid brandId)
    {
        return _mediator.CreateStream(new GetBrandAllAgentGameTypesRequest(brandId));
    }

    [HttpGet("{brandId}/valid-bet-conditions/{allAgentGameType}")]
    [Description("Get brand all agent valid bet conditions")]
    public IAsyncEnumerable<ValidBetConditionViewModel> GetValidBetConditionsAsync(Guid brandId,
        AllAgentGameType allAgentGameType)
    {
        return _mediator.CreateStream(new GetBrandAllAgentValidBetConditionsRequest(brandId, allAgentGameType));
    }

    [HttpGet("{brandId}/all-valid-bet-conditions")]
    [Description("Get all brand all agent valid bet conditions")]
    public IAsyncEnumerable<ValidBetConditionViewModel> GetAllValidBetConditionsAsync(Guid brandId)
    {
        return _mediator.CreateStream(new GetBrandAllAllAgentValidBetConditionsRequest(brandId));
    }
}