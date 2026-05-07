using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Infrastructure.Services.ExternalServices.Dtos
{
    public class AvalRequestDto
    {
        public string Phone { get; set; }
        public string Amount { get; set; }
    }
}
