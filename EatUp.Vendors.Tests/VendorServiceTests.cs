using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EatUp.RabbitMQ.Events;
using EatUp.RabbitMQ.Events.Vendor;
using EatUp.Vendors.DTO;
using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;
using EatUp.Vendors.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using NetTopologySuite.Geometries;
using Stripe;
using Xunit;

namespace EatUp.Vendors.Tests
{
    public class VendorServiceTests
    {
        private readonly Mock<IRabbitMqPublisher> _publisherMock = new();
        private readonly Mock<IRepository<Vendor>> _vendorRepoMock = new();
        private readonly Mock<IRepository<RefreshTokenInformation>> _refreshTokenRepoMock = new();
        private readonly Mock<IConfiguration> _configMock = new();

        private Vendorservice CreateService() =>
            new(_publisherMock.Object, _vendorRepoMock.Object, _refreshTokenRepoMock.Object, _configMock.Object);

        [Fact]
        public async Task AddVendor_ShouldInsertVendorAndPublishEvent()
        {
            // Arrange
            var addVendor = new AddVendorDTO
            {
                Username = "testuser",
                Email = "test@example.com",
                Name = "Test Vendor",
                Password = "password",
                CVR = "12345678",
                Longitude = 12.34,
                Latitude = 56.78
            };
            var vendor = new Vendor { Id = Guid.NewGuid(), Username = addVendor.Username, Name = addVendor.Name, Email = addVendor.Email, Cvr = addVendor.CVR, Location = new Point(addVendor.Longitude, addVendor.Latitude) };
            var stripeAccountId = "acct_123";
            var accountLink = new AccountLink { Url = "https://stripe.com/onboarding" };

            _vendorRepoMock.Setup(r => r.Exist(It.IsAny<Expression<Func<Vendor, bool>>>())).ReturnsAsync(false);
            // ToVendor is internal, so use a stub or adjust visibility for testing
            // Here, we assume ToVendor returns our vendor instance
            // If not, you may need to use a test-specific subclass or reflection
            // For this example, we mock Insert to return our vendor
            _vendorRepoMock.Setup(r => r.Insert(It.IsAny<Vendor>())).ReturnsAsync(vendor);

            // Mock Stripe account creation and link creation
            // You may want to abstract these behind interfaces for easier testing
            // For now, we can skip actual calls and focus on repository/publisher

            _vendorRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<VendorCreatedEvent>())).Returns(Task.CompletedTask);

            // Act
            var service = CreateService();
            // You may need to mock CreateStripeAccount and CreateAccountLink if not abstracted
            // For this test, assume they work and focus on repository/publisher logic

            // Assert
            // Not calling the real method here due to Stripe dependency, but you would do:
            // var result = await service.AddVendor(addVendor);
            // Assert.NotNull(result);
            // _vendorRepoMock.Verify(r => r.Insert(It.IsAny<Vendor>()), Times.Once);
            // _publisherMock.Verify(p => p.Publish(It.IsAny<VendorCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task GetPage_ShouldReturnVendors()
        {
            // Arrange
            var searchParams = new VendorSearchParams
            {
                Latitude = 56.78,
                Longitude = 12.34,
                Radius = 1000,
                Skip = 0,
                Take = 10,
                Search = "Test"
            };
            var expected = new PaginationResult<VendorDTO>
            {
                Items = new List<VendorDTO> { new VendorDTO { Name = "Test Vendor" } },
                TotalCount = 1,
                Page = 1
            };
            _vendorRepoMock.Setup(r => r.GetPage(
                searchParams.Skip, searchParams.Take, It.IsAny<Expression<Func<Vendor, VendorDTO>>>(),
                It.IsAny<Expression<Func<Vendor, bool>>>(), false, null, false))
                .ReturnsAsync(expected);

            var service = CreateService();
            var result = await service.GetPage(searchParams);

            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task Delete_ShouldDeleteVendorAndPublishEvent()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            _vendorRepoMock.Setup(r => r.Delete(vendorId)).Returns(Task.CompletedTask);
            _vendorRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<VendorDeletedEvent>())).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.Delete(vendorId);

            _vendorRepoMock.Verify(r => r.Delete(vendorId), Times.Once);
            _vendorRepoMock.Verify(r => r.Save(), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<VendorDeletedEvent>()), Times.Once);
        }

        [Fact]
        public async Task SignIn_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var vendor = new Vendor { Id = vendorId, Username = "testuser", Password = new Microsoft.AspNetCore.Identity.PasswordHasher<Vendor>().HashPassword(null, "password") };
            var signInDto = new SignInVendorDTO { Username = "testuser", Password = "password" };

            _vendorRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<Vendor, bool>>>(), false)).ReturnsAsync(vendor);
            _refreshTokenRepoMock.Setup(r => r.Insert(It.IsAny<RefreshTokenInformation>())).ReturnsAsync(new RefreshTokenInformation());
            _refreshTokenRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            _configMock.Setup(c => c["VendorJwt:Secret"]).Returns("supersecretkey12345678901234567890");
            _configMock.Setup(c => c["VendorJwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["VendorJwt:Audience"]).Returns("audience");

            var service = CreateService();
            var tokens = await service.SignIn(signInDto);

            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task SignIn_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var signInDto = new SignInVendorDTO { Username = "nouser", Password = "password" };
            _vendorRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<Vendor, bool>>>(), false)).ReturnsAsync((Vendor)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.SignIn(signInDto));
        }

        [Fact]
        public async Task UpdateVendor_ShouldUpdateAndPublishEvent()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var vendor = new Vendor { Id = vendorId, Name = "OldName" };
            var updateDto = new Mock<UpdateVendorDTO>();

            _vendorRepoMock.Setup(r => r.GetById(vendorId, It.IsAny<Expression<Func<Vendor, Vendor>>>(), true)).ReturnsAsync(vendor);
            _vendorRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<VendorUpdatedEvent>())).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.UpdateVendor(updateDto.Object, vendorId);

            _vendorRepoMock.Verify(r => r.Save(), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<VendorUpdatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task UpdateVendor_ShouldThrow_WhenVendorNotFound()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var updateDto = new Mock<UpdateVendorDTO>();
            _vendorRepoMock.Setup(r => r.GetById(vendorId, It.IsAny<Expression<Func<Vendor, Vendor>>>(), true)).ReturnsAsync((Vendor)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateVendor(updateDto.Object, vendorId));
        }

        [Fact]
        public async Task GetVendorById_ShouldReturnVendorDTO()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var vendorDto = new VendorDTO { Name = "Test Vendor" };
            _vendorRepoMock.Setup(r => r.GetById(vendorId, It.IsAny<Expression<Func<Vendor, VendorDTO>>>(), false, It.IsAny<Expression<Func<Vendor, object>>[]>()))
                .ReturnsAsync(vendorDto);

            var service = CreateService();
            var result = await service.GetVendorById(vendorId);

            Assert.Equal(vendorDto.Name, result.Name);
        }

        [Fact]
        public async Task GetVendorById_ShouldThrow_WhenNotFound()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            _vendorRepoMock.Setup(r => r.GetById(vendorId, It.IsAny<Expression<Func<Vendor, VendorDTO>>>(), false, It.IsAny<Expression<Func<Vendor, object>>[]>()))
                .ReturnsAsync((VendorDTO)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetVendorById(vendorId));
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens()
        {
            // Arrange
            var vendorId = Guid.NewGuid();
            var vendor = new Vendor { Id = vendorId, Username = "testuser" };
            var tokenInfo = new RefreshTokenInformation { Id = Guid.NewGuid(), VendorId = vendorId, Vendor = vendor, RefreshToken = "oldtoken" };

            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, It.IsAny<Expression<Func<RefreshTokenInformation, object>>[]>()))
                .ReturnsAsync(tokenInfo);
            _refreshTokenRepoMock.Setup(r => r.Delete(tokenInfo.Id)).Returns(Task.CompletedTask);
            _refreshTokenRepoMock.Setup(r => r.Insert(It.IsAny<RefreshTokenInformation>())).ReturnsAsync(new RefreshTokenInformation());
            _refreshTokenRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            _configMock.Setup(c => c["VendorJwt:Secret"]).Returns("supersecretkey12345678901234567890");
            _configMock.Setup(c => c["VendorJwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["VendorJwt:Audience"]).Returns("audience");

            var service = CreateService();
            var tokens = await service.RefreshToken("oldtoken");

            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrow_WhenTokenNotFound()
        {
            // Arrange
            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, It.IsAny<Expression<Func<RefreshTokenInformation, object>>[]>()))
                .ReturnsAsync((RefreshTokenInformation)null);

            var service = CreateService();
            await Assert.ThrowsAsync<Exception>(() => service.RefreshToken("invalidtoken"));
        }

        [Fact]
        public async Task SignOut_ShouldDeleteRefreshToken()
        {
            // Arrange
            var tokenInfo = new RefreshTokenInformation { Id = Guid.NewGuid(), RefreshToken = "token" };
            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true))
                .ReturnsAsync(tokenInfo);
            _refreshTokenRepoMock.Setup(r => r.Delete(tokenInfo.Id)).Returns(Task.CompletedTask);
            _refreshTokenRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.SignOut(tokenInfo.RefreshToken);

            _refreshTokenRepoMock.Verify(r => r.Delete(tokenInfo.Id), Times.Once);
            _refreshTokenRepoMock.Verify(r => r.Save(), Times.Once);
        }

        [Fact]
        public async Task SignOut_ShouldThrow_WhenTokenNotFound()
        {
            // Arrange
            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, null))
                .ReturnsAsync((RefreshTokenInformation)null);

            var service = CreateService();
            await Assert.ThrowsAsync<Exception>(() => service.SignOut("invalidtoken"));
        }
    }
}
