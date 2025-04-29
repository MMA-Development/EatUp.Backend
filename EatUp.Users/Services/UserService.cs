using EatUp.Users.Models;
using EatUp.Users.Repositories;

namespace EatUp.Users.Services
{
    public class UserService(IBaseRepository<User> repository) : IUserService
    {
        public void AddUser(User meal)
        {
            EnsureMeal(meal);
            repository.Insert(meal);
            repository.Save();
        }

        public async Task<PaginationResult<User>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }

        public void EnsureMeal(User meal)
        {
         
        }

        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }
    }
}
