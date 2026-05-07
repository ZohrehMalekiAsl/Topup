using Topup.Application.Interfaces.Infra;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogService<Worker> _logger;
       
        private readonly IServiceScopeFactory _scopeFactory;

        public Worker(ILogService<Worker> logger,  IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var repo = scope.ServiceProvider.GetRequiredService<IQueueMessageConsumer>();
            //to do log, exception
            repo.StartConsuming(stoppingToken);
            return Task.CompletedTask;
        }
    }
}
