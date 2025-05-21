using EatUp.RabbitMQ.Events.Meals;

namespace EatUp.Hangfire.Jobs
{
    public class PerformMealHardResyncJob(IRabbitMqPublisher publisher): IJob
    {
        public static string JobId => "PerformMealHardResyncJob";

        public async Task ExecuteAsync()
        {
            var @event = new PerformMealHardResyncEvent();
            await publisher.Publish(@event);
        }
    }
}
