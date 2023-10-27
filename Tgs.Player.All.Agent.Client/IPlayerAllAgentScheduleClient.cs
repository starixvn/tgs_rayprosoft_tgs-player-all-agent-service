namespace Tgs.Player.All.Agent.Client;

public interface IPlayerAllAgentScheduleClient
{
    Task CreateDailyPlayerAllAgentRebatesAsync();
    Task CreateWeeklyPlayerAllAgentRebatesAsync();
    Task CreateMonthlyPlayerAllAgentRebatesAsync();
    Task ManualCreateDailyPlayerAllAgentRebatesAsync(DateOnly date);
    Task ManualCreateWeeklyPlayerAllAgentRebatesAsync(DateOnly date);
    Task DistributeRebatesAsync(DateOnly fromDate, DateOnly toDate);
}