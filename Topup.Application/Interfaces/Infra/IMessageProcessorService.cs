using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Interfaces.Infra
{
    public interface IMessageProcessorService
    {
        Task ProccessPendingAsync(CancellationToken cancellationToken);
    }
}
