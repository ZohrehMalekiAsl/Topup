using Microsoft.Extensions.Logging;
using Topup.Application.Dtos.ExternalServices;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;
using Topup.Domain.Models;
using Topup.Domain.Repositories;

namespace Topup.Infrastructure.Services
{
    public class MessageProcessorService : IMessageProcessorService
    {
        private readonly IChargeRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService<MessageProcessorService> _logger;
        private readonly IExternalApiService _hamrahAvalApi;
        private readonly IAppSetting _appSetting;

        public MessageProcessorService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageProcessorService> logger, IExternalApiService hamrahAvalApi,
            IAppSetting appSetting)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _hamrahAvalApi = hamrahAvalApi;
            _appSetting = appSetting;
        }
        public async Task ProccessPendingAsync(CancellationToken cancellationToken)
        {
            var requests = await _repository.GetRequestByStatus(Status.Pending.ToString());
            if (requests != null && requests.Count != 0)
            {
                try
                {
                    foreach (var record in requests)
                    {
                        var request = new AvalRequestModel
                        {
                            Amount = record.Amount,
                            Phone = record.PhoneNumber
                        };
                        var response = await _hamrahAvalApi.ChargeAsync(request);

                        if (response.Status == Status.Failed.ToString())
                        {
                            record.RetryCount++;
                        }
                        if (response.Status == Status.Failed.ToString() && record.RetryCount >= short.Parse(_appSetting.RetryCount))
                        {
                            record.Status = Status.Failed.ToString();
                        }
                        else if (response.Status == Status.Success.ToString())
                        {
                            record.Status = response.Status;
                        }
                        _repository.Update(record);
                    }
                    await _unitOfWork.SaveAysnc();
                }
                catch (Exception ex)
                {
                    _logger.LogData(LogLevel.Error, ex.Message);
                }
            }
        }
    }
}
