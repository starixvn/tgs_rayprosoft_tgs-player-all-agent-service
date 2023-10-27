// ReSharper disable CheckNamespace

using Microsoft.EntityFrameworkCore;
using Tgs.Player.All.Agent.Domain.Interfaces;
using Tgs.Player.All.Agent.Repository;
using Tgs.Player.All.Agent.Repository.Entities;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepository(this IServiceCollection services,
        Action<IServiceProvider, DbContextOptionsBuilder> dbContextConfigure)
    {
        return services
            .AddDbContext<tgsContext>(dbContextConfigure)
            .AddTransient<IPlayerAllAgentRepository, PlayerAllAgentRepository>()
            .AddTransient<IBrandAllAgentRepository, BrandAllAgentRepository>();
    }
}