using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CoreFra.Common;
using EasyCaching.Core.Interceptor;

namespace CoreFra.Caching
{
    public static class CacheKeyGenerator
    {
        private const char LinkChar = ':';
        private const char SepChar = ',';
        private const char UnderlineChar = '_';
        private const char OpenBracketChar = '{';
        private const char CloseBracketChar = '}';

        /// <summary>
        /// args: parametername, parametervalue
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static string GenerateCacheKey(MethodInfo methodInfo, Dictionary<string, object> args)
        {

            var methodArguments = args?.Any() == true
                ? args.Select(x => GenerateCacheKey(x.Key, x.Value))
                : new[] { "0" };

            return GenerateCacheKey(methodInfo, methodArguments);
        }

        private static string GenerateCacheKey(string parameterName, object value)
        {
            if (value == null)
                return $"{parameterName}{LinkChar}{OpenBracketChar}{null}{CloseBracketChar}";

            if (value.IsBasicType())
                return $"{parameterName}{LinkChar}{OpenBracketChar}{value.GetType()}{SepChar}{value}{CloseBracketChar}";

            if (value is IEnumerable enumerable)
                return $"{parameterName}{LinkChar}{OpenBracketChar}{typeof(IEnumerable)}{SepChar}{GenerateCacheKey(enumerable.Cast<object>())}{CloseBracketChar}";

            var customTypeKeyBuilder = new StringBuilder();
            var customType = value.GetType();
            var properties = customType.GetProperties();
            foreach (var property in properties)
            {
                var info = property.GetValue(value);
                var name = property.Name;

                customTypeKeyBuilder = customTypeKeyBuilder.Append(GenerateCacheKey(name, info) + SepChar);
            }

            customTypeKeyBuilder.Remove(customTypeKeyBuilder.Length - 1, 1);

            return $"{parameterName}{LinkChar}{OpenBracketChar}{customTypeKeyBuilder}{CloseBracketChar}";
        }

        private static string GenerateCacheKey(MethodInfo methodInfo, IEnumerable<string> parameters)
        {
            var cacheKeyPrefix = GetCacheKeyPrefix(methodInfo);

            var builder = new StringBuilder();
            builder.Append(cacheKeyPrefix);
            builder.Append(string.Join(SepChar.ToString(), parameters));
            builder.Append(CloseBracketChar);
            return builder.ToString();
        }

        private static string GetCacheKeyPrefix(MethodInfo methodInfo)
        {
            var typeName = methodInfo.DeclaringType?.Name;
            var methodName = methodInfo.Name;

            return $"{typeName}{UnderlineChar}{methodName}{LinkChar}{OpenBracketChar}";
        }

        

        private static string GenerateCacheKey(IEnumerable<object> parameter)
        {
            if (parameter == null) return string.Empty;
            return "[" + string.Join(",", parameter) + "]";
        }

        
    }
}