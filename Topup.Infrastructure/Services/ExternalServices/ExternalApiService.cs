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

        public ExternalApiService(HttpClient httpClient, ILogService<ExternalApiService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://api.example.com/charge");
        }

        public async Task<AvalResponseModel> ChargeAsync(AvalRequestModel model)
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

            try
            {

                var response = await _httpClient.PostAsync("/api/v1/charge", jsonContent);
                var avalResponse = new AvalResponseModel();
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStreamAsync();
                    var responseDto = await JsonSerializer.DeserializeAsync<AvalResponseDto>(responseStream);
                    if (responseDto != null)
                    {
                        avalResponse.Status = Status.Success.ToString();
                        avalResponse.Id = model.Id;

                        var log = new LogData<AvalRequestModel, AvalResponseModel>
                        {
                            CorrelationId = model.Id.ToString(),
                            RequestInfo = model,
                            ResponseInfo = avalResponse
                        };
                        _logger.LogData(LogLevel.Information, log);
                    }
                    else
                    {
                        avalResponse.Status = Status.Failed.ToString();
                        avalResponse.Id = model.Id;
                        var log = new LogData<AvalRequestModel, AvalResponseModel>
                        {
                            CorrelationId = model.Id.ToString(),
                            RequestInfo = model,
                            ResponseInfo = avalResponse
                        };
                        _logger.LogData(LogLevel.Error, log);
                    }
  
                    return avalResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var log = new LogData<AvalRequestModel, string>
                    {
                        CorrelationId = model.Id.ToString(),
                        RequestInfo = model,
                        ResponseInfo = errorContent
                    };
                    _logger.LogData(LogLevel.Error, log);
                    return new AvalResponseModel
                    {
                        Status = Status.Failed.ToString(),
                        Id = model.Id
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                var log = new LogData<AvalRequestModel, string>
                {
                    CorrelationId = model.Id.ToString(),
                    RequestInfo = model,
                    ResponseInfo = ex.Message
                };
                _logger.LogData(LogLevel.Error, log);
                return new AvalResponseModel
                {
                    Status = Status.Failed.ToString(),
                    Id = model.Id
                };
            }
            catch (JsonException ex)
            {
                var log = new LogData<AvalRequestModel, string>
                {
                    CorrelationId = model.Id.ToString(),
                    RequestInfo = model,
                    ResponseInfo = ex.Message
                };
                _logger.LogData(LogLevel.Error, log);
                return new AvalResponseModel
                {
                    Status = Status.Failed.ToString(),
                    Id = model.Id
                };
            }
        }
    }
}
