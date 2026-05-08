using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Formatting.Compact;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;
using Topup.Infrastructure;
using Topup.Infrastructure.Context;
using Topup.Infrastructure.Repositories;
using Topup.Infrastructure.Services;
using Topup.Infrastructure.Services.ExternalServices;
using Topup.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IMessageConsumerService, MessageConsumerService>();
builder.Services.AddScoped<IMessageProcessorService, MessageProcessorService>();
builder.Services.AddScoped<IMessagePublisherService, MessagePublisherService>();
builder.Services.AddScoped<IChargeRequestRepository, ChargeRequestRepository>();
builder.Services.AddHttpClient<IExternalApiService, ExternalApiService>();
builder.Services.AddSingleton(typeof(ILogService<>), typeof(LogService<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddSingleton<IAppSetting, AppSettingsProvider>();
builder.Services.Configure<AppSetting>(builder.Configuration.GetSection("Settings"));
builder.Services.AddDbContextPool<ApplicationDbContext>((sp, options) =>
{
    var settings = sp.GetRequiredService<IAppSetting>();
    options.UseSqlServer(settings.ConnectionString);           
});
builder.Services.AddHostedService<MessageConsumerWorker>();
builder.Services.AddHostedService<MessageProcessorWorker>();
builder.Services.AddHostedService<MessagePublisherWorker>();
builder.Services.AddSerilog();
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Filter.ByIncludingOnly(e =>
    e.Properties.ContainsKey("SourceContext") &&
    e.Properties["SourceContext"].ToString().Contains("LogService"))

    .WriteTo.File(new RenderedCompactJsonFormatter(),
        path: "logs/Topup-.txt",
        rollingInterval: RollingInterval.Day
          )
    .CreateLogger();

var host = builder.Build();
host.Run();
