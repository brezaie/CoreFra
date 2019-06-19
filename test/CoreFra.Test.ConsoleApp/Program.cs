﻿using System;
using System.IO;
using CoreFra.Caching;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CoreFra.Test.ConsoleApp
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            Configuration = new ConfigurationBuilder().Build();
            var serviceProvider = new ServiceCollection()
                .AddSingleton<IConfiguration>(Configuration)
                .AddTransient(typeof(ICacheProvider), typeof(CacheManagerProvider))
                .AddSingleton<CacheManagerInterceptor>()
                .AddMvcCore(x => x.Filters.Add(typeof(CacheManagerInterceptor)));

            var test = new CachingTest();
            var res = test.TestString("hiiiiiiii");


            Console.WriteLine("Hello World!");
        }


    }
}
