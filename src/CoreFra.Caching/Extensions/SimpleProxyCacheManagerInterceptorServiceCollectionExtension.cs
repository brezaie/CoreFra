using System;
using Microsoft.Extensions.DependencyInjection;
using SimpleProxy.Extensions;

namespace CoreFra.Caching.Extensions
{
    public static class SimpleProxyCacheManagerInterceptorServiceCollectionExtension
    {
        public static IServiceCollection ConfigureSimpleProxyCacheManagerInterceptor(this IServiceCollection services)
        {
            try
            {
                services.AddSingleton<ICacheProvider, CacheManagerProvider>();

                services.EnableSimpleProxy(p =>
                {
                    p.AddInterceptor<CacheManagerAttribute, CacheManagerInterceptor>();
                });

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