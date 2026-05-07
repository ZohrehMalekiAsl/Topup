using Microsoft.Extensions.Options;
using Topup.Application.Interfaces.Infra;

namespace Topup.Infrastructure
{

    public class AppSettingsProvider: IAppSetting
    {
        private readonly IOptionsMonitor<AppSetting> _options;

        public AppSettingsProvider(IOptionsMonitor<AppSetting> options)
        {
            _options = options;
        }


        public string ConnectionString => _options.CurrentValue.ConnectionStrings.DefaultConnection;

        public string ConsumerMqHostName => _options.CurrentValue.RabbitMq.ConsumerMqHostName;
        public string ConsumerQueue => _options.CurrentValue.RabbitMq.ConsumerQueue;
    }

}
