using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Infrastructure.Services
{
    public class MessagePublisherService : IMessagePublisherService
    {
        private readonly IChargeRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService<MessageConsumerService> _logService;
        private readonly IAppSetting _appSetting;

        public MessagePublisherService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageConsumerService> logService, IAppSetting appSetting)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logService = logService;
            _appSetting = appSetting;
        }
        public async Task StartPublishing(CancellationToken ct)
        {
            var factory = new ConnectionFactory { HostName = _appSetting.SuccessPublisherMqHostName };
            using var connection = factory.CreateConnectionAsync();
            using var Channel = connection.Result.CreateChannelAsync();

            //var failfactory = new ConnectionFactory { HostName = _appSetting.FailPublisherMqHostName };
            //using var failconnection = factory.CreateConnectionAsync();
            //using var failChannel = failconnection.Result.CreateChannelAsync();

            var requests = await _repository.GetRequestByStatus(Status.Success.ToString());
            foreach (var record in requests)
            {
                var body = JsonSerializer.Serialize(new
                {
                    record.TerminalId,
                    record.Status,
                    record.Amount,
                    record.Id
                });
                var request = Encoding.UTF8.GetBytes(body);
                await Channel.Result.BasicPublishAsync(
                    exchange: "",
                    routingKey: _appSetting.SuccessPublisherQueue,
                    body: request,
                    ct
                    );
                record.Status = Status.Processing.ToString();
                _repository.Update(record);
            }
            await _unitOfWork.SaveAysnc();

        }
    }
}
