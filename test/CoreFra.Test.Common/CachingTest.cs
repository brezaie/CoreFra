using CoreFra.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public class CachingTest : ICachingTest
    {
        [CacheManager]
        public string TestString(string hello)
        {
            return "Hello";
        }
    }
}