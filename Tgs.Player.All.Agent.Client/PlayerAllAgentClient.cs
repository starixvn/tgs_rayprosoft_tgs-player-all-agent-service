namespace Tgs.Player.All.Agent.Client;

public class PlayerAllAgentClient : IPlayerAllAgentClient
{
    private readonly HttpClient _client;

    public PlayerAllAgentClient(HttpClient client)
    {
        _client = client;
    }

    public async Task CreateDailyPlayerAllAgentRebatesAsync()
    {
        var response = await _client.PostAsync("/api/playerAllAgent/daily-player-all-agent", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task CreateWeeklyPlayerAllAgentRebatesAsync()
    {
        var response = await _client.PostAsync("/api/playerAllAgent/weekly-player-all-agent", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task CreateMonthlyPlayerAllAgentRebatesAsync()
    {
        var response = await _client.PostAsync("/api/playerAllAgent/monthly-player-all-agent", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task ManualCreateDailyPlayerAllAgentRebatesAsync(DateOnly date)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/manual-daily-player-all-agent?date={date}", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task ManualCreateWeeklyPlayerAllAgentRebatesAsync(DateOnly date)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/manual-weekly-player-all-agent?date={date}", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task DistributeRebatesAsync(DateOnly fromDate, DateOnly toDate)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/distribute-rebates?fromDate={fromDate}&toDate={toDate}", null)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}