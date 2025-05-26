using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EatUp.Meals.DTO;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.Meals.Services;
using EatUp.RabbitMQ.Events.Meals;
using Moq;
using Xunit;

namespace EatUp.Meals.Tests
{
    public class MealServiceTests
    {
        private readonly Mock<IRepository<Meal>> _mealRepoMock = new();
        private readonly Mock<IRepository<VendorProjection>> _vendorRepoMock = new();
        private readonly Mock<IRabbitMqPublisher> _publisherMock = new();

        private MealService CreateService() =>
            new(_mealRepoMock.Object, _vendorRepoMock.Object, _publisherMock.Object);

        [Fact]
        public async Task GetPage_ShouldReturnPaginationResult()
        {
            // Arrange
            var searchParams = new MealSearchParamsDTO { Take = 10, Skip = 0 };
            var expected = new PaginationResult<MealDTO>
            {
                Items = new List<MealDTO> { new MealDTO { Id = Guid.NewGuid() } },
                TotalCount = 1,
                Page = 1
            };
            _mealRepoMock.Setup(r => r.GetPage(
                searchParams.Skip, searchParams.Take, It.IsAny<Expression<Func<Meal, MealDTO>>>(),
                It.IsAny<Expression<Func<Meal, bool>>>(), false, null, false))
                .ReturnsAsync(expected);

            // Act
            var service = CreateService();
            var result = await service.GetPage(searchParams);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Delete_ShouldDeleteMealAndPublishEvent()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var vendorId = Guid.NewGuid();
            var meal = new Meal { Id = mealId, VendorId = vendorId };
            _mealRepoMock.Setup(r => r.GetById(mealId, false, false)).ReturnsAsync(meal);

            // Act
            var service = CreateService();
            await service.Delete(mealId, vendorId);

            // Assert
            _mealRepoMock.Verify(r => r.Delete(mealId), Times.Once);
            _mealRepoMock.Verify(r => r.Save(), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<MealDeletedEvent>()), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowIfVendorMismatch()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var vendorId = Guid.NewGuid();
            var meal = new Meal { Id = mealId, VendorId = Guid.NewGuid() };
            _mealRepoMock.Setup(r => r.GetById(mealId, false, false)).ReturnsAsync(meal);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Delete(mealId, vendorId));
        }

        [Fact]
        public async Task UpdateMeal_ShouldThrowIfVendorMismatch()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var vendorId = Guid.NewGuid();
            var meal = new Meal { Id = mealId, VendorId = Guid.NewGuid() };
            var updateDto = new Mock<UpdateMealDTO>();
            _mealRepoMock.Setup(r => r.GetById(mealId, true, false)).ReturnsAsync(meal);

            var service = CreateService();

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.UpdateMeal(mealId, vendorId, updateDto.Object));
        }

        [Fact]
        public async Task GetMeal_ShouldReturnMealDto()
        {
            // Arrange
            var mealId = Guid.NewGuid();
            var mealDto = new MealDTO { Id = mealId };
            _mealRepoMock.Setup(r => r.GetById(mealId, It.IsAny<Expression<Func<Meal, MealDTO>>>(), false, It.IsAny<Expression<Func<Meal, object>>[]>()))
                .ReturnsAsync(mealDto);

            // Act
            var service = CreateService();
            var result = await service.GetMeal(mealId);

            // Assert
            Assert.Equal(mealId, result.Id);
        }
    }
}
