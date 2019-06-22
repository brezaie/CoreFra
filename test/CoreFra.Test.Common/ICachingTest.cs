using System;
using System.Collections.Generic;
using CoreFra.Caching;
using CoreFra.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreFra.Test.ConsoleApp
{
    public interface ICachingTest
    {
        string TestString(string hello, List<int> intList, DateTime? date, PagedCollection<string> st);
    }
}