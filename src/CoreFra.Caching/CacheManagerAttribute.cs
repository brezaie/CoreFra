using System;

namespace CoreFra.Caching
{
    public class CacheManagerAttribute : Attribute
    {
        public TimeSpan TimeToLive { get; set; }
    }
}