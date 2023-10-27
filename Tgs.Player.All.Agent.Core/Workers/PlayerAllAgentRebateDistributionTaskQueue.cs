using System.Threading.Channels;
using Tgs.Player.All.Agent.Domain.BackgroundQueue;

namespace Tgs.Player.All.Agent.Core.Workers;

public interface IPlayerAllAgentRebateDistributionTaskQueue
{
    ValueTask QueuePlayerRebateDistributionAsync(
        PlayerAllAgentRebateDistributionQueueModel model);

    ValueTask<PlayerAllAgentRebateDistributionQueueModel> DequeueAsync(
        CancellationToken cancellationToken);
}

public class PlayerAllAgentRebateDistributionTaskQueue : IPlayerAllAgentRebateDistributionTaskQueue
{
    private readonly Channel<PlayerAllAgentRebateDistributionQueueModel> _queue;

    public PlayerAllAgentRebateDistributionTaskQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true
        };
        _queue = Channel
            .CreateUnbounded<PlayerAllAgentRebateDistributionQueueModel>(options);
    }

    public async ValueTask QueuePlayerRebateDistributionAsync(
        PlayerAllAgentRebateDistributionQueueModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        await _queue.Writer.WriteAsync(model);
    }

    public async ValueTask<PlayerAllAgentRebateDistributionQueueModel>
        DequeueAsync(
            CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}