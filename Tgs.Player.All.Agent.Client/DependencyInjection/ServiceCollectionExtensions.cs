using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Tgs.Player.All.Agent.Client.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPlayerAllAgentClient(
        this IServiceCollection services,
        Action<PlayerAllAgentOptions, IServiceProvider> configure)
    {
        return services
            .AddOptions<PlayerAllAgentOptions>()
            .Configure(configure)
            .Services
            .AddHttpClient<IPlayerAllAgentScheduleClient, PlayerAllAgentScheduleClient>((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<PlayerAllAgentOptions>>().Value;

                c.BaseAddress = options.Url;
            })
            .Services
            .AddHttpClient<IBrandAllAgentClient, BrandAllAgentClient>((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<PlayerAllAgentOptions>>().Value;

                c.BaseAddress = options.Url;
            })
            .Services;
    }
}