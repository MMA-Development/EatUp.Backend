using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;
using EatUp.Users.Models;
using EatUp.Users.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EatUp.Users.EventHandlers
{
    public class PerformUserFavoriteHardResyncEventHandler(IRepository<UserFavorite> repository, IRabbitMqPublisher publisher) : IEventHandler<PerformUserFavoriteHardResyncEvent>
    {
        public async Task HandleAsync(PerformUserFavoriteHardResyncEvent _)
        {
            var all = await (await repository.GetAll(false)).ToArrayAsync();
            foreach (var userFavorite in all)
            {
                var @event = ToEvent(userFavorite);
                await publisher.Publish(@event);
            }
        }

        private static UserFavoriteHardResyncEvent ToEvent(UserFavorite userFavorite)
        {
            return new UserFavoriteHardResyncEvent
            {
                Id = userFavorite.Id,
                DeletedAt = userFavorite.DeletedAt,
                UserId = userFavorite.UserId,
                MealId = userFavorite.MealId,
            };
        }
    }
}
