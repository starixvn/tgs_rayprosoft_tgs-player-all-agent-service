using System.Threading.Channels;
using Tgs.Player.All.Agent.Domain.Models;

namespace Tgs.Player.All.Agent.Core.Workers;

public interface IPlayerAllAgentRebateTaskQueue
{
    ValueTask QueuePlayerRebateAsync(
        PlayerAllAgentRebateQueueModel model);

    ValueTask<PlayerAllAgentRebateQueueModel> DequeueAsync(
        CancellationToken cancellationToken);
}

public class PlayerAllAgentRebateTaskQueue : IPlayerAllAgentRebateTaskQueue
{
    private readonly Channel<PlayerAllAgentRebateQueueModel> _queue;

    public PlayerAllAgentRebateTaskQueue()
    {
        var options = new UnboundedChannelOptions
        {
            SingleReader = true
        };
        _queue = Channel
            .CreateUnbounded<PlayerAllAgentRebateQueueModel>(options);
    }

    public async ValueTask QueuePlayerRebateAsync(
        PlayerAllAgentRebateQueueModel model)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        await _queue.Writer.WriteAsync(model);
    }

    public async ValueTask<PlayerAllAgentRebateQueueModel>
        DequeueAsync(
            CancellationToken cancellationToken)
    {
        var workItem = await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}