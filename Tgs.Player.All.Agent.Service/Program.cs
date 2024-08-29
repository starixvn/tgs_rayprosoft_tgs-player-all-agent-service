using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Exporter;
using OpenTelemetry.Instrumentation.AspNetCore;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.Player.All.Agent.Core.DependencyInjection;
using Tgs.Player.All.Agent.Core.Workers;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseSentry();

builder.Configuration
    .AddJsonFile("/config/appsettings.json", true)
    .AddEnvironmentVariables();

builder.Host.ConfigureServices((context, services) => services
    .AddHostedService<PlayerAllAgentRebateBackground>()
    .AddSingleton<IPlayerAllAgentRebateTaskQueue, PlayerAllAgentRebateTaskQueue>()
    .AddHostedService<PlayerAllAgentRebateDistributionBackground>()
    .AddSingleton<IPlayerAllAgentRebateDistributionTaskQueue, PlayerAllAgentRebateDistributionTaskQueue>());

builder.Services
    .AddCore()
    .AddRepository((_, dbOptionsBuilder) => dbOptionsBuilder
        .UseNpgsql("name=Tgs"))
    .AddCashServiceClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.BaseUrl = config.GetValue<Uri>("PlayerAllAgentRebate:CashBaseUrl")!;
    })
    .AddPlayerBucketClient(httpClientBuilder => httpClientBuilder
        .ConfigureHttpClient(httpClient => httpClient.BaseAddress =
            builder.Configuration.GetValue<Uri>("PlayerAllAgentRebate:PlayerBucketBaseUrl")))
    .AddPlayerServiceClient((options, sp) =>
    {
        var config = sp.GetRequiredService<IConfiguration>();

        options.BaseUrl = config.GetValue<Uri>("PlayerAllAgentRebate:PlayerServiceBaseUrl")!;
    });

builder.Services
    .AddMessageQueue()
    .AddNatsMessageQueue(natsConfig => natsConfig
        .ConfigQueueOptions((options, sp) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();

            options.Url = config.GetConnectionString("Nats")!;
        })
    )
    .AddNatsGlobPatternExchange("*");

builder.Logging
    .AddOpenTelemetry(options =>
    {
        options.IncludeFormattedMessage = true;
        options.ParseStateValues = true;
        _ = options.AddFluentdForwardExporter(fluentOptions =>
        {
            var nodeIp = builder.Configuration.GetValue<string>("Otel:NodeIp");
            fluentOptions.Host = nodeIp!;
            fluentOptions.Tag = builder.Configuration.GetValue<string>("Otel:ServiceName")!;
            fluentOptions.UseMessagePack();
        });
    });

builder.Services
    .AddOptions<OtlpExporterOptions>()
    .Configure<IConfiguration>((options, conifg) =>
    {
        var nodeIp = conifg.GetValue<string>("NODE_IP");

        if (!string.IsNullOrWhiteSpace(nodeIp))
            options.Endpoint = new Uri($"http://{nodeIp}:4317");

        conifg.GetSection("Opentelemetry:Tracing").Bind(options);
    });

builder.Services.AddOpenTelemetry()
    .ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")))
    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
        .AddSource("Tgs.*")
        .AddSource("TGS.*")
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
        .AddProfileViewExporter()
        .AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")))
    .Services
    .Configure<AspNetCoreInstrumentationOptions>(options => options.Filter =
        httpContext => !httpContext.Request.Path.HasValue ||
                       (!httpContext.Request.Path.StartsWithSegments("/swagger") &&
                        !httpContext.Request.Path.StartsWithSegments("/health") &&
                        !httpContext.Request.Path.StartsWithSegments("/profiler/") &&
                        !new[] { "html", "ico", "png", "jpg" }.Any(
                            ext => httpContext.Request.Path.Value.EndsWith($".{ext}"))));
builder.Services.AddProfileViewer();

builder.Services.AddHealthChecks();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwaggerUI();
app.MapControllers();
app.MapSwagger();
app.MapHealthChecks("/health");
app.MapHealthChecks("/");

SentrySdk.CaptureMessage($"Started result Player-All-Agent-Service at {DateTime.Now.ToString()}");

app.Run();