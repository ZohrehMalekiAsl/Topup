using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Interfaces.Infra;
using Topup.Domain.Interfaces;
using Topup.Domain.Models;

namespace Topup.Infrastructure.Services
{
    public class LogService<TService> : ILogService<TService>
    {
        private readonly ILogger<LogService<TService>> _logger;
        public LogService(ILogger<LogService<TService>> logger)
        {
            _logger = logger;
        }

        public bool LogData<TRequest, TResponse>(LogLevel logLevel, LogData<TRequest, TResponse> logData)
        {
            _logger.Log(logLevel, "Request handled {@LogData}", logData);
            return true;
        }

        public bool LogData(LogLevel logLevel, string logData)
        {
            _logger.Log(logLevel, "Request handled {@LogData}", logData);
            return true;
        }
    }
}
