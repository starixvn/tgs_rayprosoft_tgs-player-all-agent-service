using System.ComponentModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Tgs.Player.All.Agent.Domain.Enums;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlayerAllAgentController : ControllerBase
{
    private readonly IMediator _mediator;

    public PlayerAllAgentController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("daily-player-all-agent")]
    [Description("create daily player all agent rebates")]
    public Task CreateDailyPlayerAllAgentRebatesAsync()
    {
        var previousDay = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).AddDays(-1);

        var reportDate = new DateTimeOffset(previousDay.Year,
            previousDay.Month,
            previousDay.Day,
            0,
            0,
            0,
            previousDay.Offset);

        return _mediator.Send(new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Daily,
            reportDate.UtcDateTime,
            reportDate.UtcDateTime));
    }

    [HttpPost("weekly-player-all-agent")]
    [Description("create weekly player all agent rebates")]
    public Task CreateWeeklyPlayerAllAgentRebatesAsync()
    {
        var previousDay = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0)).AddDays(-1);

        var reportDate = new DateTimeOffset(previousDay.Year,
            previousDay.Month,
            previousDay.Day,
            0,
            0,
            0,
            previousDay.Offset);

        var weekFrom = reportDate.GetMonday();

        var weekTo = reportDate.GetSunday();

        return _mediator.Send(new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Weekly,
            weekFrom.UtcDateTime,
            weekTo.UtcDateTime));
    }

    [HttpPost("monthly-player-all-agent")]
    [Description("create monthly player all agent rebates")]
    public Task CreateMonthlyPlayerAllAgentRebatesAsync()
    {
        var now = DateTimeOffset.Now.ToOffset(new TimeSpan(8, 0, 0));

        var reportDate = new DateTimeOffset(now.Year,
            now.Month,
            now.Day,
            0,
            0,
            0,
            now.Offset);

        var monthFrom = reportDate.AddMonths(-1).GetFirstDayOfMonth();

        var monthTo = reportDate.AddMonths(-1).GetLastDayOfMonth();

        return _mediator.Send(new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Monthly,
            monthFrom.UtcDateTime,
            monthTo.UtcDateTime));
    }

    [HttpPost("manual-daily-player-all-agent")]
    [Description("Manual create daily player all agent rebates")]
    public Task ManualCreateDailyPlayerAllAgentRebatesAsync([FromQuery] DateOnly date)
    {
        if (!DateTimeOffset.TryParse($"{date} 00:00 +08:00", out var result))
            return Task.CompletedTask;

        var rebateDate = new DateTimeOffset(result.Year,
            result.Month,
            result.Day,
            0,
            0,
            0,
            result.Offset);

        return _mediator.Send(new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Daily,
            rebateDate.UtcDateTime,
            rebateDate.UtcDateTime));
    }

    [HttpPost("manual-weekly-player-all-agent")]
    [Description("Manual create weekly player all agent rebates")]
    public Task ManualCreateWeeklyPlayerAllAgentRebatesAsync([FromQuery] DateOnly date)
    {
        if (!DateTimeOffset.TryParse($"{date} 00:00 +08:00", out var result))
            return Task.CompletedTask;

        var rebateDate = new DateTimeOffset(result.Year,
            result.Month,
            result.Day,
            0,
            0,
            0,
            result.Offset);

        var weekFrom = rebateDate.GetMonday();
        var weekTo = rebateDate.GetSunday();

        var today = DateTime.UtcNow.Date;

        if (today <= weekTo) return Task.CompletedTask;

        return _mediator.Send(new CreatePlayerAllAgentDistributionRequest(SettlementIntervalType.Weekly,
            weekFrom.UtcDateTime,
            weekTo.UtcDateTime));
    }

    [HttpPost("distribute-rebates")]
    [Description("Distribute rebates")]
    public Task DistributeRebatesAsync([FromQuery] DateOnly fromDate, [FromQuery] DateOnly toDate)
    {
        if (!DateTimeOffset.TryParse($"{fromDate} 00:00 +08:00", out var resultFrom) ||
            !DateTimeOffset.TryParse($"{toDate} 00:00 +08:00", out var resultTo))
            return Task.CompletedTask;

        var rebateFromDate = new DateTimeOffset(resultFrom.Year,
            resultFrom.Month,
            resultFrom.Day,
            0,
            0,
            0,
            resultFrom.Offset);

        var rebateToDate = new DateTimeOffset(resultTo.Year,
            resultTo.Month,
            resultTo.Day,
            0,
            0,
            0,
            resultTo.Offset);

        return _mediator.Send(
            new PlayerAllAgentManualDistributionRequest(rebateFromDate.UtcDateTime, rebateToDate.UtcDateTime));
    }
}