using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Tgs.Player.All.Agent.Client.Models;

namespace Tgs.Player.All.Agent.Client;

internal class BrandAllAgentClient : IBrandAllAgentClient
{
    private readonly HttpClient _client;

    private readonly JsonSerializerOptions _jsonSerializerOptions =
        new() { PropertyNameCaseInsensitive = true, DefaultBufferSize = 128 };

    public BrandAllAgentClient(HttpClient client)
    {
        _client = client;
    }

    public async Task UpdateSwitchAsync(UpdateSwitchRequest request)
    {
        using var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var response = await _client.PatchAsync(
            "/api/brandAllAgent/switch",
            jsonContent);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateGameTypeSwitchAsync(UpdateGameTypeSwitchRequest request)
    {
        using var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var response = await _client.PutAsync("/api/brandAllAgent/game-type",
            jsonContent);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateSettlementIntervalAsync(UpdateSettlementIntervalRequest request)
    {
        using var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var response = await _client.PutAsync("/api/brandAllAgent/settlement-interval",
            jsonContent);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task UpdateValidBetConditionAsync(UpdateValidBetConditionRequest request)
    {
        using var jsonContent = new StringContent(
            JsonSerializer.Serialize(request, _jsonSerializerOptions),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        using var response = await _client.PutAsync("/api/brandAllAgent/valid-bet-conditions",
            jsonContent);

        _ = response.EnsureSuccessStatusCode();
    }

    public async Task<BrandAllAgent> GetBrandAllAgentAsync(GetBrandAllAgentRequest request)
    {
        var response = await _client
            .GetAsync($"/api/brandAllAgent/{request.BrandId}/switch", HttpCompletionOption.ResponseContentRead)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        return JsonSerializer.Deserialize<BrandAllAgent>(
            await response.Content.ReadAsStringAsync(),
            _jsonSerializerOptions)!;
    }

    public async IAsyncEnumerable<GameTypeModel> GetGameTypesAsync(GetGameTypesRequest request)
    {
        var response = await _client
            .GetAsync($"/api/brandAllAgent/{request.BrandId}/game-types", HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var gameTypes = JsonSerializer.DeserializeAsyncEnumerable<GameTypeModel>(
            responseStream,
            _jsonSerializerOptions);

        await foreach (var gameType in gameTypes) yield return gameType!;
    }

    public async IAsyncEnumerable<ValidBetConditionModel> GetValidBetConditionsAsync(
        GetValidBetConditionsRequest request)
    {
        var response = await _client
            .GetAsync($"/api/brandAllAgent/{request.BrandId}/valid-bet-conditions/{request.AllAgentGameType}",
                HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var validBetConditions = JsonSerializer.DeserializeAsyncEnumerable<ValidBetConditionModel>(
            responseStream,
            _jsonSerializerOptions);

        await foreach (var validBetCondition in validBetConditions) yield return validBetCondition!;
    }

    public async IAsyncEnumerable<ValidBetConditionModel> GetAllValidBetConditionsAsync(GetAllValidBetConditionsRequest request)
    {
        var response = await _client
            .GetAsync($"/api/brandAllAgent/{request.BrandId}/all-valid-bet-conditions",
                HttpCompletionOption.ResponseHeadersRead)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var responseStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

        var validBetConditions = JsonSerializer.DeserializeAsyncEnumerable<ValidBetConditionModel>(
            responseStream,
            _jsonSerializerOptions);

        await foreach (var validBetCondition in validBetConditions) yield return validBetCondition!;
    }
}