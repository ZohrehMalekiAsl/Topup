using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Interfaces.Infra
{
    public interface IMessagePublisherService
    {
        Task StartPublishing(CancellationToken ct, string status);
    }
}
