using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Domain.Entities
{
    public class ChargeRequest: IEntity
    {
        public Guid Id { get; set; }
        public string Amount { get; set; }
        public string PhoneNumber { get; set; }
        public string TerminalId { get; set; }
        public string Status { get; set; }
        public short RetryCount { get; set; }
        public string? SystemTrace { get; set; }
        public DateTime RequestTimestamp { get; set; }
    }
}
