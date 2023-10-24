namespace Tgs.Player.All.Agent.Client;

public interface IPlayerAllAgentClient
{
    Task CreateDailyPlayerAllAgentRebatesAsync();
    Task CreateWeeklyPlayerAllAgentRebatesAsync();
    Task CreateMonthlyPlayerAllAgentRebatesAsync();
    Task ManualCreateDailyPlayerAllAgentRebatesAsync(string date);
    Task ManualCreateWeeklyPlayerAllAgentRebatesAsync(string date);
    Task DistributeRebatesAsync(string fromDate, string toDate);
}