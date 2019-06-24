using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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

        public static string ReverseString(this string str)
        {
            return new string(((IEnumerable<char>)str.ToCharArray()).Reverse<char>().ToArray<char>());
        }

        public static string RemoveNoise(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return string.Empty;
            }
            return str.Replace("‏", "");
        }

        public static string GetDescription(this Enum value)
        {
            DescriptionAttribute customAttribute = Attribute.GetCustomAttribute((MemberInfo)value.GetType().GetField(value.ToString()), typeof(DescriptionAttribute)) as DescriptionAttribute;
            return customAttribute == null ? value.ToString() : customAttribute.Description;
        }

        public static T GetValueFromDescription<T>(string description)
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                throw new InvalidOperationException();
            foreach (FieldInfo field in type.GetFields())
            {
                DescriptionAttribute customAttribute = Attribute.GetCustomAttribute((MemberInfo)field, typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (customAttribute != null)
                {
                    if (customAttribute.Description == description)
                        return (T)field.GetValue((object)null);
                }
                else if (field.Name == description)
                    return (T)field.GetValue((object)null);
            }
            throw new ArgumentException("Value is not valid", nameof(description));
        }

    }
}