
using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Hangfire.Jobs
{
    public class PerformUserFavoriteHardResyncJob(IRabbitMqPublisher publisher) : IJob
    {
        public static string JobId => "PerformUserFavoriteHardResyncJob";
        public async Task ExecuteAsync()
        {
            var @event = new PerformUserFavoriteHardResyncEvent();
            await publisher.Publish(@event);
        }
    }
}
