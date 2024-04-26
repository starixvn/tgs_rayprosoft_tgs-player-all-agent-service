using Tgs.Player.All.Agent.Client.Models;

namespace Tgs.Player.All.Agent.Client;

public interface IBrandAllAgentClient
{
    Task UpdateSwitchAsync(UpdateSwitchRequest request);
    Task UpdateGameTypeSwitchAsync(UpdateGameTypeSwitchRequest request);
    Task UpdateSettlementIntervalAsync(UpdateSettlementIntervalRequest request);
    Task UpdateValidBetConditionAsync(UpdateValidBetConditionRequest request);
    Task<BrandAllAgent> GetBrandAllAgentAsync(GetBrandAllAgentRequest request);
    IAsyncEnumerable<GameTypeModel> GetGameTypesAsync(GetGameTypesRequest request);
    IAsyncEnumerable<ValidBetConditionModel> GetValidBetConditionsAsync(GetValidBetConditionsRequest request);
    IAsyncEnumerable<ValidBetConditionModel> GetAllValidBetConditionsAsync(GetAllValidBetConditionsRequest request);
}