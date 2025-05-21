using Hangfire;

namespace EatUp.Hangfire.Jobs
{
    public static class ConfigureJobs
    {
        public static void Configure(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<PerformVendorHardResyncJob>(PerformVendorHardResyncJob.JobId, job => job.ExecuteAsync(), Cron.Never);
            RecurringJob.AddOrUpdate<PerformUserHardResyncJob>(PerformUserHardResyncJob.JobId, job => job.ExecuteAsync(), Cron.Never);
        }
    }
}
