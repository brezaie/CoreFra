using System.Collections.Generic;
using CoreFra.Caching;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public interface ICachingTest
    {
        string TestString(string hello, List<int> intList);
    }
}