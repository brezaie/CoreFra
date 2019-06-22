using System;

namespace CoreFra.Common
{
    public static class Utility
    {
        public static bool IsBasicType(this object value)
        {
            if (value is string || value is int || value is long || value is decimal || value is float ||
                value is bool || value is double
                || value is DateTime || value is DateTimeOffset || value is uint || value is char || value is byte ||
                value is ulong
                || value is short)
                return true;

            return false;
        }
    }
}