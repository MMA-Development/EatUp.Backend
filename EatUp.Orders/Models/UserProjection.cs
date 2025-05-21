using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Orders.Models
{
    public class UserProjection: BaseEntity
    {
        public string Fullname { get; set; }
        public object Email { get; private set; }

        internal static UserProjection FromCreatedEvent(UserCreatedEvent @event)
        {
            return new UserProjection()
            {
                Fullname = @event.Fullname,
                Id = @event.Id,
                Email = @event.Email
            };
        }

        internal static void MergeUpdatedEvent(UserProjection existing, UserUpdatedEvent @event)
        {
            existing.Fullname = @event.Fullname;
            existing.Email = @event.Email;
        }
    }
}
