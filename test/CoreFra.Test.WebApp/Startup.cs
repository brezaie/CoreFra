﻿using System;
using AspectCore.Extensions.DependencyInjection;
using CoreFra.Caching;
using CoreFra.Test.ConsoleApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspectCore.Injector;
using CoreFra.Caching.Extensions;
using CoreFra.Domain;
using CoreFra.Logging;
using CoreFra.Logging.Extensions;
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

        public void ConfigureServices(IServiceCollection services)
        {
            try
            {
                services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

                services.AddSingleton<ICustomLogger, SeriLogger>();

                #region Auditor

                var elasticSearchUrl = Configuration["ElasticSearch:Url"];
                var elasticSearchUsername = Configuration["ElasticSearch:UserName"];
                var elasticSearchPassword = Configuration["ElasticSearch:Password"];
                var elasticSearchIndexName = Configuration["ElasticSearch:IndexFormat"];
                
                var elasticSetting = new ElasticSetting
                {
                    ConnectionString = elasticSearchUrl,
                    Username = elasticSearchUsername,
                    Password = elasticSearchPassword,
                    IndexFormat = elasticSearchIndexName
                };

                services.ConfigureSimpleProxyAuditorInterceptor().AddElasticProvider(elasticSetting);

                #endregion

                #region Caching

                services.ConfigureSimpleProxyCacheManagerInterceptor().AddCacheManager();

                #endregion

                #region SimpleProxy

                services.AddTransientWithProxy<ICachingTest, CachingTest>();

                #endregion

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

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
