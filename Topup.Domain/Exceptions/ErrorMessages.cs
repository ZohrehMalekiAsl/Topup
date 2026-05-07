using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topup.Domain.Enums;

namespace Topup.Domain.Exceptions
{
    public static class ErrorMessages
    {
        private static readonly Dictionary<BusinessErrorCode, string> _messages = new()
        {
            { BusinessErrorCode.UnsuccessfullCharge, "Author Not Found"},
            { BusinessErrorCode.MessageNotFound, "UserNotFound"},
            { BusinessErrorCode.UserDeactivated, "UserDeactivated"},
            { BusinessErrorCode.TimeOut, "BookNotFound"  },
            { BusinessErrorCode.MessageAlreadyExists, "User Already Exists"}

        };
        public static bool GetMessage(BusinessErrorCode code, out string message) =>
        _messages.TryGetValue(code, out message);
    }
}
