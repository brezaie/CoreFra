using System;
using System.Collections.Generic;

namespace CoreFra.Caching
{
    public interface ICacheProvider
    {
        object Get(string key);
        object Get(string key, string region);

        void Remove(string key);
        void Remove(string key, string region);

        void ClearRegion(string region);
        void ClearAll();

        void AddOrUpdate(string key, object value, TimeSpan? timeToLive = null);
        void AddOrUpdate(string key, object value, string region, TimeSpan? timeToLive = null);
    }
}