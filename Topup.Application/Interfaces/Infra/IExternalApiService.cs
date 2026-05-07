using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Application.Dtos.ExternalServices;

namespace Topup.Application.Interfaces.Infra
{
    public interface IExternalApiService
    {
        Task<AvalResponseModel> ChargeAsync(AvalRequestModel model);
    }
}
