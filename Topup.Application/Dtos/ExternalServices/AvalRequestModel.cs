using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Application.Dtos.ExternalServices
{
    public class AvalRequestModel
    {
        public string Phone { get; set; }
        public string Amount { get; set; }
        public Guid Id { get; set; }
    }
}
