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
        var response = await _client.PostAsync("/api/playerAllAgent/daily-player-all-agent", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task CreateWeeklyPlayerAllAgentRebatesAsync()
    {
        var response = await _client.PostAsync("/api/playerAllAgent/weekly-player-all-agent", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task CreateMonthlyPlayerAllAgentRebatesAsync()
    {
        var response = await _client.PostAsync("/api/playerAllAgent/monthly-player-all-agent", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task ManualCreateDailyPlayerAllAgentRebatesAsync(string date)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/manual-daily-player-all-agent/{date}", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task ManualCreateWeeklyPlayerAllAgentRebatesAsync(string date)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/manual-weekly-player-all-agent/{date}", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task DistributeRebatesAsync(string fromDate, string toDate)
    {
        var response = await _client.PostAsync($"/api/playerAllAgent/distribute-rebates/{fromDate}/{toDate}", null).ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();
    }
}