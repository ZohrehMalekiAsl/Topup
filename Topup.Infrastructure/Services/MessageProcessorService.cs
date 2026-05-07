using Topup.Application.Dtos.ExternalServices;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;
using Topup.Domain.Repositories;

namespace Topup.Infrastructure.Services
{
    public class MessageProcessorService : IMessageProcessorService
    {
        private readonly IChargeRequestRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogService<MessageProcessorService> _logService;
        private readonly IExternalApiService _hamrahAvalApi;

        public MessageProcessorService(IChargeRequestRepository repository, IUnitOfWork unitOfWork,
            ILogService<MessageProcessorService> logService, IExternalApiService hamrahAvalApi)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logService = logService;
            _hamrahAvalApi = hamrahAvalApi;
        }
        public async Task ProccessPendingAsync(CancellationToken cancellationToken)
        {
            var requests = await _repository.GetRequestByStatus(Status.Pending.ToString());
            if (requests != null)
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
                            record.RetryCount = record.RetryCount ++;
                        }
                        else if(response.Status == Status.Failed.ToString() && record.RetryCount>=3)
                        {
                            record.Status = Status.Failed.ToString();
                        }
                        else if(response.Status == Status.Success.ToString())
                        {
                            record.Status = response.Status;
                        }
                        _repository.Update(record);
                    }
                    await _unitOfWork.SaveAysnc();
                }
                catch
                {
                }
            }
        }
    }
}
