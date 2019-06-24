using System;
using SimpleProxy.Attributes;

namespace CoreFra.Caching
{
    public class CacheAttribute : MethodInterceptionAttribute
    {
        public int Day { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
    }
}