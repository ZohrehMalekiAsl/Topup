using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Interfaces.Infra
{
    public interface IAppSetting
    {
        string ConnectionString { get; }
        string ConsumerMqHostName { get; }
        string ConsumerQueue { get; }
    }
}
