using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using Tgs.Player.All.Agent.Repository.Entities;

namespace Tgs.Player.All.Agent.IntegrationTest;

extern alias service;

internal class TestApplication : WebApplicationFactory<service::Program>
{
    private readonly Action<IWebHostBuilder>? _webHostConfigure;

    public TestApplication(Action<IWebHostBuilder>? webHostConfigure = null)
    {
        _webHostConfigure = webHostConfigure;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _ = builder
            .ConfigureAppConfiguration(builder => builder.Sources.Clear())
            .ConfigureLogging(logging => logging.ClearProviders().AddConsole());

        _ = builder.ConfigureServices(services => services
            .AddSingleton<IConfigureOptions<OtlpExporterOptions>>(
                sp => new ConfigureNamedOptions<OtlpExporterOptions>(
                    Options.DefaultName,
                    exporterOptions =>
                    {
                        exporterOptions.TimeoutMilliseconds = 0;
                        exporterOptions.ExportProcessorType = ExportProcessorType.Batch;
                        exporterOptions.Endpoint = new Uri("http://otel.rpgp.cc:4317");
                    }))
            .AddFakeNatsMessageQueue()
        .Replace(ServiceDescriptor.Singleton(sp =>
        {
                var dbContextOptionsBuilder = new DbContextOptionsBuilder<tgsContext>();
                _ = dbContextOptionsBuilder
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .ConfigureWarnings(x => x.Ignore(
                        InMemoryEventId.TransactionIgnoredWarning));


                return new tgsContext(dbContextOptionsBuilder.Options);
            })));

        _webHostConfigure?.Invoke(builder);
    }
}