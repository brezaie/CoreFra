using System;
using System.IO;
using CoreFra.Agent;
using CoreFra.Caching;
using CoreFra.Domain;
using CoreFra.Logging;
using CoreFra.Test.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Topshelf;

namespace CoreFra.Test.ConsoleApp
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }

        static void Main(string[] args)
        {
            //Configuration = new ConfigurationBuilder().Build();
            //var serviceProvider = new ServiceCollection()
            //    .AddSingleton<IConfiguration>(Configuration)
            //    .AddTransient(typeof(ICacheProvider), typeof(CacheManagerProvider))
            //    .AddSingleton<CacheInterceptor>()
            //    .AddMvcCore(x => x.Filters.Add(typeof(CacheInterceptor)));


            #region Job

            var jobsetting = new JobSetting
            {
                JobName = "somename",
                TriggerName = "sometrigger",
                IntervalInMinutes = 1
            };

            var logger = new SeriLogger(new Logger<SeriLogger>(new LoggerFactory()));

            var host = HostFactory.Run(x =>
            {
                x.Service<QuartzScheduler<TestJob>>(s =>
                {
                    s.WhenStarted(service => service.Start(jobsetting));
                    s.WhenStopped(service => service.Stop());
                    s.ConstructUsing(() => new QuartzScheduler<TestJob>(logger));
                });

                x.RunAsLocalSystem()
                    .DependsOnEventLog()
                    .StartAutomatically()
                    .EnableServiceRecovery(rc => rc.RestartService(1));

                x.SetDisplayName(jobsetting.JobName);
                x.SetServiceName(jobsetting.JobName);
                x.SetDescription(jobsetting.JobName);

            });

            #endregion

        }


    }
}
