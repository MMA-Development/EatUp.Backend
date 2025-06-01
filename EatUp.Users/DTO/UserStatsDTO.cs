using System.Linq.Expressions;
using EatUp.Users.Models;

namespace EatUp.Users.DTO
{
    public class UserStatsDTO
    {
        public static int ASSUMED_WEIGHT_OF_ORDER = 1; // in kg, this is a placeholder value for calculation purposes
        public static double CO2_PER_KG = 2.5; // CO2 emissions per kg of food, this is a placeholder value for calculation purposes

        public double CO2Saved { get; set; }
        public double MoneySaved { get; set; }

        public static Expression<Func<User, UserStatsDTO>> Projection => user => new()
        {
            CO2Saved = user.Orders.Sum(x => x.Quantity) * ASSUMED_WEIGHT_OF_ORDER * CO2_PER_KG,
            MoneySaved = user.Orders.Sum(x => x.OriginalPrice - x.TotalPrice)
        };
    }
}
