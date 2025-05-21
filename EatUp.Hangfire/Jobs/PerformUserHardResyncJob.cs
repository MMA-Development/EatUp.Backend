using EatUp.RabbitMQ.Events.Users;
using EatUp.RabbitMQ.Events.Vendor;

namespace EatUp.Hangfire.Jobs
{
    public class PerformUserHardResyncJob(IRabbitMqPublisher publisher): IJob
    {
        public static string JobId => "PerformUserHardResyncJob";

        public async Task ExecuteAsync()
        {
            var @event = new PerformUserHardResyncEvent();
            await publisher.Publish(@event);
        }
    }
}
