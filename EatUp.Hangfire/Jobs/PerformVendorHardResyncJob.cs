using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Hangfire.Jobs
{
    public class PerformVendorHardResyncJob(IRabbitMqPublisher publisher): IJob
    {
        public static string JobId => "PerformVendorHardResyncJob";

        public async Task ExecuteAsync()
        {
            var @event = new PerformVendorHardResyncEvent();
            await publisher.Publish(@event);
        }
    }
}
