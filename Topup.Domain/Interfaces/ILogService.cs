using Microsoft.Extensions.Logging;
using Topup.Domain.Models;

namespace Topup.Domain.Interfaces
{
    public interface ILogService<TService>
    {
        bool LogData<TRequest, TResponse>(LogLevel logLevel, LogData<TRequest, TResponse> logData);
        bool LogData (LogLevel logLevel, string logData);
    }
}
