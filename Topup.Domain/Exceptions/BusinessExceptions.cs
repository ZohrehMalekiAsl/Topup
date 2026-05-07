using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Domain.Enums;

namespace Topup.Domain.Exceptions
{
    public class BusinessExceptions: Exception
    {
        public BusinessErrorCode Error;
        public BusinessExceptions(string message, BusinessErrorCode error) : base(message)
        {
            Error = error;
        }
    }
}
