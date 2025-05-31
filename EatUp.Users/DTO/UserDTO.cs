using EatUp.Users.Models;

namespace EatUp.Users.DTO
{
    public class UserDTO
    {
        public string Email { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string StripeCustomerId { get; set; } = null!;
        public List<FavoriteDTO> Favorites { get; set; } = [];

        internal static UserDTO FromUser(User vendor)
        {
            return new UserDTO
            {
                Email = vendor.Email,
                Username = vendor.Username,
                FullName = vendor.FullName,
                StripeCustomerId = vendor.StripeCustomerId,
                Favorites = vendor.Favorites.Select(x => new FavoriteDTO
                {
                    MealId = x.MealId,
                }).ToList()
            };
        }
    }
}
