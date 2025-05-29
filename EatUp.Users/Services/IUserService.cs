using EatUp.Users.DTO;
using EatUp.Users.Models;

namespace EatUp.Users.Services
{
    public interface IUserService
    {
        Task AddToFavorite(Guid mealId, Guid userId);
        Task AddUser(AddUserDTO adduser);
        Task Delete(Guid id);
        Task<IEnumerable<UserFavorite>> GetFavorites(Guid userId);
        Task<UserDTO> GetUserById(Guid vendorId);
        Task<UserTokens> RefreshToken(string refreshToken);
        Task<UserTokens> SignIn(SignInUserDTO singInUser);
        Task SignOut(string refreshToken);
        Task UnFavorite(Guid mealId, Guid userId);
        Task UpdateUser(UpdateUserDTO vendorDTO, Guid vendorId);
    }
}