using Abp.BackgroundJobs;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CourseFeedbackSystem.BackgroundJobs;
using CourseFeedbackSystem.Configuration;
using Hangfire;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace CourseFeedbackSystem.Web.Host.Startup
{
    [DependsOn(
       typeof(CourseFeedbackSystemWebCoreModule))]
    public class CourseFeedbackSystemWebHostModule : AbpModule
    {
        private readonly IWebHostEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public CourseFeedbackSystemWebHostModule(IWebHostEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(CourseFeedbackSystemWebHostModule).GetAssembly());
        }

        public override void PostInitialize()
        {
            // Schedule the daily feedback monitoring job
            RecurringJob.AddOrUpdate<FeedbackMonitoringJob>(
                "feedback-monitoring-job",
                job => job.ExecuteAsync(new FeedbackMonitoringJobArgs()),
                Cron.Daily(2, 0), // Run daily at 2:00 AM
                new RecurringJobOptions
                {
                    TimeZone = TimeZoneInfo.Utc
                });
        }
    }
}
