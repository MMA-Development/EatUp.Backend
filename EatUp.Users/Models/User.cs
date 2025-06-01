using System.ComponentModel.DataAnnotations.Schema;

namespace EatUp.Users.Models
{
    public class User : BaseEntity
    {
        public string Username { get; set; } = null!; 
        public string Password { get; set; } = null!; 
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? StripeCustomerId { get; set; }

        [ForeignKey("UserId")]
        public virtual List<RefreshTokenInformation> RefreshTokens { get; set; } = [];

        [ForeignKey("UserId")]
        public virtual List<UserFavorite> Favorites { get; set; } = [];

        [ForeignKey("UserId")]
        public virtual List<OrderCompletedProjection> Orders { get; set; } = [];
    }
}
