using EatUp.RabbitMQ.Events.Users;

namespace EatUp.Orders.Models
{
    public class UserProjection: BaseEntity
    {
        public string Fullname { get; set; }
        public string Email { get; private set; }
        public string? StripeCustomerId { get; internal set; }

        internal static UserProjection FromCreatedEvent(UserCreatedEvent @event)
        {
            return new UserProjection()
            {
                Fullname = @event.Fullname,
                Id = @event.Id,
                Email = @event.Email,
                StripeCustomerId = @event.StripeCustomerId,
            };
        }

        internal static UserProjection FromHardResyncEvent(UserHardResyncEvent @event) => new UserProjection()
        {
            CreatedAt = @event.CreatedAt,
            DeletedAt = @event.DeletedAt,
            UpdatedAt = @event.UpdatedAt,
            Fullname = @event.Fullname,
            Id = @event.Id,
            Email = @event.Email,
            StripeCustomerId = @event.StripeCustomerId,
        };

        internal static void HardResync(UserProjection projection, UserHardResyncEvent @event)
        {
            projection.Fullname = @event.Fullname;
            projection.Email = @event.Email;
            projection.CreatedAt = @event.CreatedAt;
            projection.DeletedAt = @event.DeletedAt;
            projection.UpdatedAt = @event.UpdatedAt;
            projection.StripeCustomerId = @event.StripeCustomerId;
        }

        internal static void MergeUpdatedEvent(UserProjection existing, UserUpdatedEvent @event)
        {
            existing.Fullname = @event.Fullname;
            existing.Email = @event.Email;
            existing.StripeCustomerId = @event.StripeCustomerId;
        }
    }
}
