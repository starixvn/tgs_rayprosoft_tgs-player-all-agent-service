using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Tgs.Player.All.Agent.Core.Workers;

namespace Tgs.Player.All.Agent.Core.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCore(this IServiceCollection services)
    {
        return services.AddMediatR(r => r.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
            .AddTransient<IPlayerAllAgentRebateTaskQueueProcessor, PlayerAllAgentRebateTaskQueueProcessor>();
    }
}