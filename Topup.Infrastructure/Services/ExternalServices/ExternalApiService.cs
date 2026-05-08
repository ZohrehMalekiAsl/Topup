using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using Topup.Application.Dtos.ExternalServices;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Enums;
using Topup.Domain.Interfaces;
using Topup.Domain.Models;
using Topup.Infrastructure.Services.ExternalServices.Dtos;

namespace Topup.Infrastructure.Services.ExternalServices
{
    public class ExternalApiService : IExternalApiService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogService<ExternalApiService> _logger;
        private readonly IAppSetting _appSetting;

        public ExternalApiService(HttpClient httpClient, ILogService<ExternalApiService> logger,
            IAppSetting appSetting)
        {
            _httpClient = httpClient;
            _logger = logger;
            _appSetting = appSetting;
            _httpClient = httpClient;
        }

        public async Task<AvalResponseModel> ChargeAsync(AvalRequestModel model)
        {
            AvalResponseModel response =new AvalResponseModel();
            response.systemTrace = model.SystemTrace;

            try
            {
                var request = new AvalRequestDto
                {
                    Amount = model.Amount,
                    Phone = model.Phone,
                };

                var jsonContent = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json");

                _httpClient.BaseAddress = new Uri(_appSetting.HamrahAvalUrl);
                var httpResponse = await _httpClient.PostAsync("/api/v1/charge", jsonContent);
                
                if (httpResponse.IsSuccessStatusCode)
                {
                    var responseStream = await httpResponse.Content.ReadAsStreamAsync();
                    var responseDto = await JsonSerializer.DeserializeAsync<AvalResponseDto>(responseStream);
                    
                    if(responseDto != null)
                    {
                        response.Status = responseDto.Status;

                        var log = new LogData<AvalRequestModel, AvalResponseDto>
                        {
                            CorrelationId = model.SystemTrace,
                            RequestInfo = model,
                            ResponseInfo = responseDto
                        };
                        _logger.LogData(LogLevel.Information, log);
                    }
                    else
                    {
                        response.Status = Status.Failed.ToString();
                        var log = new LogData<AvalRequestModel, string>
                        {
                            CorrelationId = model.SystemTrace,
                            RequestInfo = model,
                            Description = null
                        };
                        _logger.LogData(LogLevel.Error, log);
                    }
                    return response;
                }
                else
                {
                    var errorContent = await httpResponse.Content.ReadAsStringAsync();
                    var log = new LogData<AvalRequestModel, string>
                    {
                        CorrelationId = model.SystemTrace,
                        RequestInfo = model,
                        Description = errorContent
                    };
                    _logger.LogData(LogLevel.Error, log);

                    response.Status = Status.Failed.ToString();
                    return response;
                }
            }
            catch (Exception ex)
            {
                var log = new LogData<AvalRequestModel, string>
                {
                    CorrelationId = model.SystemTrace,
                    RequestInfo = model,
                    Description = ex.Message
                };
                _logger.LogData(LogLevel.Error, log);

                response.Status = Status.Failed.ToString();
                return response;
            }
        }
    }
}
