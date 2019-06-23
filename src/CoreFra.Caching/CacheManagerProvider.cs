using System;
using CacheManager.Core;
using Microsoft.Extensions.Logging;

namespace CoreFra.Caching
{
    public class CacheManagerProvider : ICacheProvider
    {
        private readonly ICacheManager<object> _cacheManager;
        
        public CacheManagerProvider()
        {
            _cacheManager =
                CacheFactory.Build<object>(
                    s =>
                        s.WithDictionaryHandle()
                            .EnablePerformanceCounters()
                            .EnableStatistics()
                            .Build()
                        );

        }


        public object Get(string key)
        {
            var value = _cacheManager.Get(key);
            return value;
        }

        public object Get(string key, string region)
        {
            var value = _cacheManager.Get(key, region);
            return value;
        }


        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        public void Remove(string key, string region)
        {
            _cacheManager.Remove(key, region);
        }

        public void ClearRegion(string region)
        {
            _cacheManager.ClearRegion(region);
        }

        public void ClearAll()
        {
            _cacheManager.Clear();
        }

        public void AddOrUpdate(string key, object value, TimeSpan? timeToLive = null)
        {
            _cacheManager.AddOrUpdate(key, value, v => value);

            if (!timeToLive.HasValue) return;


            var localTime = DateTime.Now.AddSeconds(timeToLive.Value.TotalSeconds);
            DateTimeOffset? date = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            _cacheManager.Expire(key, date.Value);
        }

        public void AddOrUpdate(string key, object value, string region, TimeSpan? timeToLive = null)
        {
            _cacheManager.AddOrUpdate(key, region, value, v => value);

            if (!timeToLive.HasValue) return;

            var localTime = DateTime.Now.AddSeconds(timeToLive.Value.TotalSeconds);
            DateTimeOffset? date = new DateTimeOffset(localTime, TimeZoneInfo.Local.GetUtcOffset(localTime));
            _cacheManager.Expire(key, region, date.Value);
        }
    }
}