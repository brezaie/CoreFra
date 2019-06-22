using System;
using System.Collections.Generic;
using CoreFra.Caching;
using CoreFra.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public class CachingTest : ICachingTest
    {
        [CacheManager(Second = 10)]
        public string TestString(string hello, List<int> intList, DateTime? date, PagedCollection<string> st)
        {
            return "Hello";
        }
    }
}