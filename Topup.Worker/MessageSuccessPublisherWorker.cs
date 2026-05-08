using Topup.Application.Interfaces.Infra;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;

namespace Topup.Worker
{
    public class MessageSuccessPublisherWorker : BackgroundService
    {
        private readonly ILogService<MessageSuccessPublisherWorker> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        public MessageSuccessPublisherWorker(ILogService<MessageSuccessPublisherWorker> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var publishService = scope.ServiceProvider.GetRequiredService<IMessagePublisherService>();

                    await publishService.StartPublishing(stoppingToken, Status.Success.ToString()) ;
                }
                catch (Exception ex)
                {
                    _logger.LogData(LogLevel.Error, ex.Message);
                }
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }
}
