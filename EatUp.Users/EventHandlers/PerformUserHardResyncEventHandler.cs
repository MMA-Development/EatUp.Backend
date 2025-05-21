using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;
using EatUp.Users.Models;
using EatUp.Users.Repositories;

namespace EatUp.Users.EventHandlers
{
    public class PerformUserHardResyncEventHandler(IRepository<User> repository, IRabbitMqPublisher publisher) : IEventHandler<PerformUserHardResyncEvent>
    {
        public async Task HandleAsync(PerformUserHardResyncEvent _)
        {
            var allUsers = (await repository.GetAll()).ToArray();
            foreach (var user in allUsers)
            {
                var @event = ToEvent(user);
                await publisher.Publish(@event);
            }
        }

        private UserHardResyncEvent ToEvent(User user) => new()
        {
            Id = user.Id,
            Fullname = user.FullName,
            Email = user.Email,
            StripeCustomerId = user.StripeCustomerId,
            CreatedAt = user.CreatedAt,
            DeletedAt = user.DeletedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}
