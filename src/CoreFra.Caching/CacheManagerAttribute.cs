using System;
using SimpleProxy.Attributes;

namespace CoreFra.Caching
{
    public class CacheManagerAttribute : MethodInterceptionAttribute
    {
        public TimeSpan TimeToLive { get; set; }
    }
}