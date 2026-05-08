using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Entities;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Infrastructure.Services
{
    public class MessageConsumerService : IMessageConsumerService
    {
        private readonly IChargeRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService<MessageConsumerService> _logger;
        private readonly IAppSetting _appSetting;

        public MessageConsumerService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageConsumerService> logger, IAppSetting appSetting)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _appSetting = appSetting;
        }

        public async Task StartConsuming(CancellationToken ct)
        {
            var factory = new ConnectionFactory
            {
                HostName = _appSetting.ConsumerMqHostName
            };

            using var connection = await factory.CreateConnectionAsync(ct);
            using var channel = await connection.CreateChannelAsync(null, ct);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                var body = Encoding.UTF8.GetString(ea.Body.ToArray());
                //var result = new ChargeRequest();
                //result.Amount = "20000";
                //result.PhoneNumber = "09126933297";
                //result.TerminalId = "05";
                //result.SystemTrace = "2";

                var result = JsonSerializer.Deserialize<ChargeRequest>(body);
                if (result is null)
                {
                    _logger.LogData(LogLevel.Error, body + " Invalid Message");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: false);
                        return;
                }

                result.Status = Status.Pending.ToString();
                    try
                    {
                        _repository.Add(result);
                        await _unitOfWork.SaveAysnc();
                        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                    }
                    catch (DbUpdateException ex)
                    {
                        if (IsDuplicateKey(ex))
                        {
                            _logger.LogData(LogLevel.Error, "Duplicate detected." + ex.ToString());

                            await channel.BasicAckAsync(ea.DeliveryTag, false);
                            return;
                        }
                        _logger.LogData(LogLevel.Error, ex.Message + " Doblicate Message");
                        await channel.BasicNackAsync(ea.DeliveryTag, false, requeue: true);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogData(LogLevel.Error, ex.Message);
                    await channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true);
                }
            };

            await channel.BasicConsumeAsync(
                queue: _appSetting.ConsumerQueue,
                autoAck: false,
                consumer: consumer,
                cancellationToken: ct
            );
        }
        private bool IsDuplicateKey(DbUpdateException ex)
        {
            var inner = ex.InnerException?.ToString();
            if (inner == null) return false;

            return inner.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || inner.Contains("unique", StringComparison.OrdinalIgnoreCase)
                || inner.Contains("UNIQUE constraint", StringComparison.OrdinalIgnoreCase);
        }

    }

}
