using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EasyCaching.Core.Interceptor;

namespace CoreFra.Caching
{
    public static class CacheKeyGenerator
    {
        private const char LinkChar = ':';

        public static string GenerateCacheKey(MethodInfo methodInfo, object[] args)
        {

            var methodArguments = args?.Any() == true
                ? args.Select(GenerateCacheKey)
                : new[] { "0" };

            return GenerateCacheKey(methodInfo, methodArguments);
        }

        private static string GenerateCacheKey(MethodInfo methodInfo, IEnumerable<string> parameters)
        {
            var cacheKeyPrefix = GetCacheKeyPrefix(methodInfo);

            var builder = new StringBuilder();
            builder.Append(cacheKeyPrefix);
            builder.Append(string.Join(LinkChar.ToString(), parameters));
            return builder.ToString();
        }

        private static string GetCacheKeyPrefix(MethodInfo methodInfo)
        {
            var typeName = methodInfo.DeclaringType?.Name;
            var methodName = methodInfo.Name;

            return $"{typeName}{LinkChar}{methodName}{LinkChar}";
        }

        private static string GenerateCacheKey(object parameter)
        {
            if (parameter == null) return string.Empty;
            if (parameter is ICachable cachable) return cachable.CacheKey;
            if (parameter is string key) return key;
            if (parameter is DateTime dateTime) return dateTime.ToString("O");
            if (parameter is DateTimeOffset dateTimeOffset) return dateTimeOffset.ToString("O");
            if (parameter is IEnumerable enumerable) return GenerateCacheKey(enumerable.Cast<object>());
            return parameter.ToString();
        }

        private static string GenerateCacheKey(IEnumerable<object> parameter)
        {
            if (parameter == null) return string.Empty;
            return "[" + string.Join(",", parameter) + "]";
        }

        
    }
}