using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleProxy.Extensions;

namespace CoreFra.Caching.Extensions
{
    public static class SimpleProxyCacheInterceptorServiceCollectionExtension
    {
        public static IServiceCollection ConfigureSimpleProxyCacheManagerInterceptor(this IServiceCollection services)
        {
            try
            {
                services.EnableSimpleProxy(p =>
                {
                    p.AddInterceptor<CacheAttribute, CacheInterceptor>();
                });

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static IServiceCollection AddCacheManager(this IServiceCollection services)
        {
            try
            {
                services.AddSingleton<ICacheProvider, CacheManagerProvider>();

                return services;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}