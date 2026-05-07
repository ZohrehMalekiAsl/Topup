using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Entities;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Infrastructure.Services
{
    public class MessageConsumerService: IMessageConsumerService
    {
        private readonly IChargeRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService<MessageConsumerService> _logService;
        private readonly IAppSetting _appSetting;

        public MessageConsumerService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageConsumerService> logService , IAppSetting appSetting)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logService = logService;
            _appSetting = appSetting;
        }

        public async Task StartConsuming(CancellationToken ct)
        {

            var factory = new ConnectionFactory { HostName = _appSetting.ConsumerMqHostName };
            using var connection = factory.CreateConnectionAsync();
            using var channel = connection.Result.CreateChannelAsync();


            var consumer = new AsyncEventingBasicConsumer(channel.Result);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var result = JsonSerializer.Deserialize<ChargeRequest>(body);
                    //var log = new LogData<ChargeRequest, >
                    //{
                    //    CorrelationId = model.Id.ToString(),
                    //    RequestInfo = model,
                    //    ResponseInfo = avalResponse
                    //};
                    //_logger.LogData(LogLevel.Information, log);
                    _repository.Add(result);
                    _unitOfWork.SaveAysnc();
                    channel.Result.BasicAckAsync(ea.DeliveryTag, false);
                }
                catch
                {
                    channel.Result.BasicNackAsync(ea.DeliveryTag, false, true);
                }
            };

            channel.Result.BasicConsumeAsync(queue: _appSetting.ConsumerQueue,
                                  autoAck: false,
                                  consumer: consumer);
        }
    }

}
