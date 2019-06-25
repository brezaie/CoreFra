using System;
using System.Collections.Specialized;
using CoreFra.Domain;
using CoreFra.Logging;
using Quartz;
using Quartz.Impl;
using Topshelf.Builders;

namespace CoreFra.Agent
{
    public class QuartzScheduler<TJob> where TJob : CustomJob
    {
        private readonly IScheduler _scheduler;
        private readonly ICustomLogger _logger;

        public QuartzScheduler(ICustomLogger logger)
        {
            _logger = logger;
            var schedulerFactory = new StdSchedulerFactory();
            _scheduler = schedulerFactory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public virtual void Start(JobSetting jobSetting)
        {
            try
            {
                _scheduler.Start().ConfigureAwait(false).GetAwaiter().GetResult();
                ScheduleJobs(jobSetting);
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public virtual void ScheduleJobs(JobSetting jobSetting)
        {
            try
            {
                var job = JobBuilder.Create<TJob>()
                    .WithIdentity(jobSetting.JobName, jobSetting.JobName)
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity(jobSetting.TriggerName, jobSetting.JobName)
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInMinutes(jobSetting.IntervalInMinutes)
                        .RepeatForever())
                    .Build();

                // Tell quartz to schedule the job using our trigger
                _scheduler.ScheduleJob(job, trigger).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

        public virtual void Stop()
        {
            try
            {
                _scheduler.Shutdown().ConfigureAwait(false).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.ErrorException(ex.Message, ex);
                throw;
            }
        }

    }
}