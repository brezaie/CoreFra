using CoreFra.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public class CachingTestClass
    {
        [TypeFilter(typeof(CacheProviderInterceptor))]
        public virtual string TestString(string hello)
        {
            return "Hello";
        }
    }
}