using EatUp.Users.Models;

namespace EatUp.Users.Services
{
    public interface IUserService
    {
        void AddUser(User meal);
        Task Delete(Guid id);
        void EnsureMeal(User meal);
        Task<PaginationResult<User>> GetPage(int skip, int take);
    }
}