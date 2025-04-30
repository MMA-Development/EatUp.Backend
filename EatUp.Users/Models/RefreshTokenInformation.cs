namespace EatUp.Users.Models
{
    public class RefreshTokenInformation: BaseEntity
    {
        public string RefreshToken { get; set; } = null!;

        public Guid UserId { get; set; }

        public User User { get; set; } = null!;

        public DateTime ExpirationDate { get; set; }

    }
}
