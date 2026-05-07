using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Interfaces;

namespace Topup.Worker
{
    public class MessageProcessorWorker : BackgroundService
    {
        private readonly ILogService<MessageConsumerWorker> _logger;

        private readonly IServiceScopeFactory _scopeFactory;

        public MessageProcessorWorker(ILogService<MessageConsumerWorker> logger, IServiceScopeFactory scopeFactory)
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
                    var proccessService = scope.ServiceProvider.GetRequiredService<IMessageProcessorService>();

                    await proccessService.ProccessPendingAsync(stoppingToken);
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
