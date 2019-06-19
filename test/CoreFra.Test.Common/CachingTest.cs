using System.Collections.Generic;
using CoreFra.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public class CachingTest : ICachingTest
    {
        [CacheManager]
        public string TestString(string hello, List<int> intList)
        {
            return "Hello";
        }
    }
}