using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topup.Domain.Enums
{
    public enum BusinessErrorCode
    {
        MessageAlreadyExists = 1001,
        UserDeactivated = 1002,
        MessageNotFound = 1003,
        TimeOut = 2001,
        UnsuccessfullCharge = 3001,
        Unknown = 9999
    }
}
