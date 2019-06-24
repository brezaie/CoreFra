using System;
using System.Collections.Generic;
using System.Globalization;

namespace CoreFra.Common
{
    public static class SafeConversionExtension
    {
        public static string SafePersianPhrase(this string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                return
                    str.SafePersianEncode().RemoveNoise()
                        .Replace((char)8204, (char)32)
                        .TrimStart()
                        .TrimEnd();
            }

            return str;
        }

        public static string SafePersianEncode(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            str = str.Replace("ي", "ی");
            str = str.Replace("ك", "ک");
            return str.Trim();
        }

        public static int SafeBoolToInt(this object i)
        {
            return i.SafeBoolean() ? 1 : 0;
        }

        public static string SafeString(this object i)
        {
            if (i != null)
                return i.ToString();
            return (string)null;
        }

        public static string SafeString(this object i, bool isEmpty)
        {
            if (i != null)
                return i.ToString();
            return string.Empty;
        }

        public static DateTime SafeDateTime(this object d)
        {
            if (d == null)
                return new DateTime(1907, 1, 1);
            DateTime result;
            DateTime.TryParse(d.SafeString(), out result);
            return result;
        }

        public static TimeSpan SafeTime(this object d)
        {
            if (d == null)
                return TimeSpan.MinValue;
            TimeSpan result;
            TimeSpan.TryParse(d.SafeString(), out result);
            return result;
        }

        public static bool SafeBoolean(this object d)
        {
            if (d == null)
                return false;
            bool.TryParse(d.SafeString(), out var result);
            return result;
        }

        public static Guid SafeGuid(this object d)
        {
            if (d == null)
                return Guid.Empty;
            Guid result;
            Guid.TryParse(d.ToString(), out result);
            return result;
        }

        public static string SafeSqlSingleQuotes(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            str = str.Replace("'", "''");
            return str;
        }

        public static long SafeLong(this object i)
        {
            return i.SafeLong(0L);
        }

        public static long SafeLong(this object i, long exceptionValue)
        {
            if (i == null)
                return exceptionValue;
            long.TryParse(i.SafeDouble().SafeString(), out exceptionValue);
            return exceptionValue;
        }

        public static float SafeFloat(this object i)
        {
            float result;
            float.TryParse(i.SafeString(), out result);
            return result;
        }

        public static double SafeDouble(this object i)
        {
            double result;
            double.TryParse(i.SafeString(), NumberStyles.Any, (IFormatProvider)CultureInfo.InvariantCulture, out result);
            return result;
        }

        public static double SafeDouble(this object i, double exceptionValue)
        {
            if (i != null)
                double.TryParse(i.SafeString().Split('.')[0], out exceptionValue);
            return exceptionValue;
        }

        public static short SafeInt16(this object i)
        {
            short result;
            short.TryParse(i.SafeString(), out result);
            return result;
        }

        public static Decimal SafeDecimal(this object i)
        {
            Decimal result;
            Decimal.TryParse(i.SafeString(), out result);
            return result;
        }

        public static TValue SafeDictionary<TKey, TValue>(
            this Dictionary<TKey, TValue> input,
            TKey key,
            TValue ifNotFound)
        {
            TValue obj;
            if (input.TryGetValue(key, out obj))
                return obj;
            return ifNotFound;
        }

        public static byte SafeByte(this object i)
        {
            byte result = 0;
            if (i != null)
                byte.TryParse(i.SafeString().Split('.')[0], out result);
            return result;
        }

        public static int SafeInt(this object i, int exceptionValue)
        {
            if (i != null)
                int.TryParse(i.SafeString().Split('.')[0], out exceptionValue);
            return exceptionValue;
        }

        public static int SafeInt(this object i)
        {
            return i.SafeInt(-1);
        }

    }
}