using System;
using CoreFra.Common;

namespace CoreFra.Logging
{
    public static class BusinessRuleHelper
    {
        public static string GetException(Exception exception)
        {
            return exception.GetType() == typeof(BusinessRuleException)
                ? ((BusinessRuleException)exception).ErrorMessage
                : RuleCodeException.OperationFailed.GetEnumDescription();
        }
    }
}