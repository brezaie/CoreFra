using System;
using AspectCore.Extensions.DependencyInjection;
using CoreFra.Caching;
using CoreFra.Test.ConsoleApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Injector;
using SimpleProxy.Extensions;

namespace CoreFra.Test.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                //services.ConfigureAspectCoreInterceptor(options => { options.CacheProviderName = "SomeCacheProviderName"; });

                //services.ConfigureAspectCoreInterceptor(options => options.CacheProviderName = "first");

                services.AddSingleton< ICacheProvider, CacheManagerProvider>();

                services.EnableSimpleProxy(p =>
                {
                    p.AddInterceptor<CacheManagerAttribute, CacheManagerInterceptor>();
                });

                services.AddTransientWithProxy<ICachingTest, CachingTest>();

                //var container = services.ToServiceContainer();
                //container.Build();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
