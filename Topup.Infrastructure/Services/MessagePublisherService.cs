using Microsoft.Extensions.Logging;
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
        private readonly ILogService<MessageConsumerService> _logger;
        private readonly IAppSetting _appSetting;

        public MessagePublisherService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageConsumerService> logger, IAppSetting appSetting)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _appSetting = appSetting;
        }
        public async Task StartPublishing(CancellationToken ct, string status)
        {
            try
            {
                string queue= string.Empty;
                string mqHostName = string.Empty;

                if (status == Status.Success.ToString())
                {
                    queue = _appSetting.SuccessPublisherQueue;
                    mqHostName = _appSetting.SuccessPublisherMqHostName;
                }
                else
                {
                    queue = _appSetting.FailPublisherQueue;
                    mqHostName = _appSetting.FailPublisherMqHostName;
                }
                var factory = new ConnectionFactory
                {
                    HostName = mqHostName
                };

                using var connection = await factory.CreateConnectionAsync(ct);
                using var channel = await connection.CreateChannelAsync(null, ct);

                var tcs = new TaskCompletionSource<bool>();
                channel.BasicAcksAsync += async (sender, ea) =>
                {
                    if (ea.Multiple == false)
                    {
                        tcs.TrySetResult(true);
                    }
                };

                channel.BasicNacksAsync += async (sender, ea) =>
                {
                    tcs.TrySetResult(false);
                };
                var requests = await _repository.GetRequestByStatus(status);

                if (requests != null && requests.Count != 0)
                {
                    foreach (var record in requests)
                    {
                        var bodyObj = new
                        {
                            record.TerminalId,
                            record.PhoneNumber,
                            record.Amount,
                            record.SystemTrace
                        };

                        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bodyObj));

                        await channel.BasicPublishAsync(
                            exchange: "",
                            routingKey: queue,
                            body: body,
                            cancellationToken: ct
                        );

                        var confirmed = await tcs.Task;
                        if (confirmed)
                        {
                            if (status == Status.Success.ToString())
                            {
                                record.Status = Status.FinishedSuccess.ToString();
                            }
                            else 
                            {
                                record.Status = Status.FinishedFailed.ToString();
                            }
                            _repository.Update(record);
                        }
                    }
                    await _unitOfWork.SaveAysnc();
                }
            }
            catch (Exception ex)
            {
                _logger.LogData(LogLevel.Error, ex.Message);
            }
        }
    }
}
