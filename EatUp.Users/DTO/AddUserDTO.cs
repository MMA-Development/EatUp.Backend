using EatUp.Users.Models;
using Microsoft.AspNetCore.Identity;

namespace EatUp.Users.DTO
{
    public class AddUserDTO
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        public User ToUser()
        {
            var user = new User
            {
                Username = Username,
                FullName = FullName,
                Email = Email,
            };

            var hasher = new PasswordHasher<User>();
            user.Password = hasher.HashPassword(user, Password);

            return user;
        }
    }
}
