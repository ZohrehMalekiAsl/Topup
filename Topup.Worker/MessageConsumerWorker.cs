using Topup.Application.Interfaces.Infra;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Worker
{
    public class MessageConsumerWorker : BackgroundService
    {
        private readonly ILogService<MessageConsumerWorker> _logger;
       
        private readonly IServiceScopeFactory _scopeFactory;

        public MessageConsumerWorker(ILogService<MessageConsumerWorker> logger,  IServiceScopeFactory scopeFactory)
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
                    var QMService = scope.ServiceProvider.GetRequiredService<IQMConsumerService>();
                    
                    await QMService.StartConsuming(stoppingToken);
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
