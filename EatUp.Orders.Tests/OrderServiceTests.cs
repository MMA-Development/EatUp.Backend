using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EatUp.Orders.DTO;
using EatUp.Orders.Models;
using EatUp.Orders.Repositories;
using EatUp.Orders.Services;
using Moq;
using Stripe;
using Xunit;

namespace EatUp.Orders.Tests
{
    public class OrderServiceTests
    {
        private readonly Mock<IBaseRepository<Order>> _orderRepoMock = new();
        private readonly Mock<IBaseRepository<MealProjection>> _mealProjRepoMock = new();
        private readonly Mock<IBaseRepository<UserProjection>> _userProjRepoMock = new();
        private readonly Mock<IBaseRepository<VendorProjection>> _vendorProjRepoMock = new();
        private readonly Mock<IRabbitMqPublisher> _rabbitMqPublisherMock = new();

        private OrderService CreateService() =>
            new(_orderRepoMock.Object, _mealProjRepoMock.Object, _userProjRepoMock.Object, _vendorProjRepoMock.Object, _rabbitMqPublisherMock.Object);

        [Fact]
        public async Task GetPageForVendor_ReturnsPaginationResult()
        {
            var vendorId = Guid.NewGuid();
            var @params = new OrdersForVendorParams { Skip = 0, Take = 10, Search = "user" };
            var expected = new PaginationResult<OrderDTO>
            {
                Items = new List<OrderDTO> { new OrderDTO { Id = Guid.NewGuid() } },
                TotalCount = 1,
                Page = 1
            };
            _orderRepoMock.Setup(r => r.GetPage(
                @params.Skip, @params.Take, It.IsAny<Expression<Func<Order, OrderDTO>>>(),
                It.IsAny<Expression<Func<Order, bool>>>(), false, null, false))
                .ReturnsAsync(expected);

            var service = CreateService();
            var result = await service.GetPageForVendor(@params, vendorId);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task GetPageForUser_ReturnsPaginationResult()
        {
            var userId = Guid.NewGuid();
            var expected = new PaginationResult<OrderDTO>
            {
                Items = new List<OrderDTO> { new OrderDTO { Id = Guid.NewGuid() } },
                TotalCount = 1,
                Page = 1
            };
            _orderRepoMock.Setup(r => r.GetPage(
                0, 10, It.IsAny<Expression<Func<Order, OrderDTO>>>(),
                It.IsAny<Expression<Func<Order, bool>>>(), false, null, false))
                .ReturnsAsync(expected);

            var service = CreateService();
            var result = await service.GetPageForUser(0, 10, userId);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Delete_DeletesOrderAndSaves()
        {
            var orderId = Guid.NewGuid();
            _orderRepoMock.Setup(r => r.Delete(orderId)).Returns(Task.CompletedTask);
            _orderRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.Delete(orderId);

            _orderRepoMock.Verify(r => r.Delete(orderId), Times.Once);
            _orderRepoMock.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public async Task CreateOrderRequest_ThrowsIfAnyProjectionIsNull()
        {
            var request = new CreateOrderRequest
            {
                FoodPackageId = Guid.NewGuid(),
                VendorId = Guid.NewGuid(),
                UserId = Guid.NewGuid()
            };

            _mealProjRepoMock.Setup(r => r.GetById(request.FoodPackageId, false, false, null)).ReturnsAsync((MealProjection)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.CreateOrderRequest(request));
        }

        [Fact]
        public async Task HandlePaymentIntentSucceeded_ThrowsIfNull()
        {
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.HandlePaymentIntentSucceeded(null));
        }

        [Fact]
        public async Task HandlePaymentIntentSucceeded_ThrowsIfOrderIdMissing()
        {
            var paymentIntent = new PaymentIntent { Metadata = new Dictionary<string, string>() };
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.HandlePaymentIntentSucceeded(paymentIntent));
        }

        [Fact]
        public async Task HandlePaymentIntentSucceeded_ThrowsIfOrderNotFound()
        {
            var orderId = Guid.NewGuid();
            var paymentIntent = new PaymentIntent { Metadata = new Dictionary<string, string> { { "order_id", orderId.ToString() } } };
            _orderRepoMock.Setup(r => r.GetById(orderId, true, false)).ReturnsAsync((Order)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.HandlePaymentIntentSucceeded(paymentIntent));
        }


        [Fact]
        public async Task HandlePaymentIntentFailed_ThrowsIfNull()
        {
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentNullException>(() => service.HandlePaymentIntentFailed(null));
        }

        [Fact]
        public async Task HandlePaymentIntentFailed_ThrowsIfOrderIdMissing()
        {
            var paymentIntent = new PaymentIntent { Metadata = new Dictionary<string, string>() };
            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.HandlePaymentIntentFailed(paymentIntent));
        }

        [Fact]
        public async Task HandlePaymentIntentFailed_ThrowsIfOrderNotFound()
        {
            var orderId = Guid.NewGuid();
            var paymentIntent = new PaymentIntent { Metadata = new Dictionary<string, string> { { "order_id", orderId.ToString() } } };
            _orderRepoMock.Setup(r => r.GetById(orderId, true, false, null)).ReturnsAsync((Order)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.HandlePaymentIntentFailed(paymentIntent));
        }
    }
}
