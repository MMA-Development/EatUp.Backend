using EatUp.Users.Models;

namespace EatUp.Users.DTO
{
    public class UpdateUserDTO
    {
        public string Email { get; set; } = null!;
        public string FullName { get; set; } = null!;

        internal void Merge(User vendorFromDb)
        {
            vendorFromDb.Email = Email;
            vendorFromDb.FullName = FullName;
        }
    }
}
