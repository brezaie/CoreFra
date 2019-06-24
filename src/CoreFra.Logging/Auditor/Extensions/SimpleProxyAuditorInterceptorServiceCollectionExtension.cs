using System;
using CoreFra.Domain;
using Microsoft.Extensions.DependencyInjection;
using SimpleProxy.Extensions;

namespace CoreFra.Logging.Extensions
{
    public static class SimpleProxyAuditorInterceptorServiceCollectionExtension
    {
        public static IServiceCollection ConfigureSimpleProxyAuditorInterceptor(this IServiceCollection services,
            ElasticSetting elasticSetting)
        {
            try
            {
                if (elasticSetting != null)
                    services.AddTransient<IAuditorProvider>(s => new ElasticAuditorProvider(elasticSetting));

                services.EnableSimpleProxy(p => { p.AddInterceptor<AuditorAttribute, AuditorInterceptor>(); });

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