using System;
using System.Linq;
using System.Reflection;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Injector;
using EasyCaching.Core.Configurations;
using EasyCaching.Core.Interceptor;
using EasyCaching.Interceptor.AspectCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CoreFra.Caching.Extensions
{
    public static class AspectCoreInterceptorServiceCollectionExtension
    {

        public static IServiceProvider ConfigureAspectCoreInterceptor(this IServiceCollection services,
            Action<CachingInterceptorOptions> options) =>
            services.ConfigureAspectCoreInterceptor(x => { }, options);

        /// <summary>
        /// Configures the AspectCore interceptor.
        /// </summary>
        /// <returns>The aspect core interceptor.</returns>
        /// <param name="services">Services.</param>
        /// <param name="action">Action.</param>
        /// <param name="options">Easycaching Interceptor config</param>
        public static IServiceProvider ConfigureAspectCoreInterceptor(this IServiceCollection services
            , Action<IServiceContainer> action, Action<CachingInterceptorOptions> options)
        {
            try
            {
                services.Configure(options);

                var container = services.ToServiceContainer();

                action(container);

                return container.Configure(config =>
                {
                    bool all(MethodInfo x) => x.CustomAttributes.Any(data =>
                        typeof(CacheManagerInterceptor).GetTypeInfo().IsAssignableFrom(data.AttributeType));

                    //config.Interceptors.AddTyped<CacheManagerInterceptor>(Predicates.ForNameSpace("*"));
                }).Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}