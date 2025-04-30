using EatUp.Users.DTO;

namespace EatUp.Users.Services
{
    public interface IUserService
    {
        Task AddUser(AddUserDTO adduser);
        Task Delete(Guid id);
        Task<UserDTO> GetUserById(Guid vendorId);
        Task<UserTokens> RefreshToken(string refreshToken);
        Task<UserTokens> SignIn(SignInUserDTO singInUser);
        Task SignOut(string refreshToken);
        Task UpdateUser(UpdateUserDTO vendorDTO, Guid vendorId);
    }
}