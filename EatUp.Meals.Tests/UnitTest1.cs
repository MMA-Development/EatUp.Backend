using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.Meals.Services;
using Moq;

namespace EatUp.microservice.tst
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {

        }

        [Fact]
        public void LastAvailablePickupIsEarlierThenNow()
        {
            // Arrange
            var meal = new Meal
            {
                LastAvailablePickup = DateTime.UtcNow.AddHours(-1)
            };

            var mealService = new MealService(new Mock<IBaseRepository<Meal>>().Object);

            // Act
            Action act = () => mealService.EnsureMeal(meal);

            //Assert
            Assert.Throws<ArgumentException>(act);
        }

        [Fact]
        public void LastAvailablePickupIsNotEarlierThenNow()
        {
            // Arrange
            var meal = new Meal
            {
                LastAvailablePickup = DateTime.UtcNow.AddHours(1)
            };

            var mealService = new MealService(new Mock<IBaseRepository<Meal>>().Object);

            // Act
            Action act = () => mealService.EnsureMeal(meal);

            //Assert
            act();
        }
    }
}