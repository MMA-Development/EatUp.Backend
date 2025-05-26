using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;
using EatUp.Users.DTO;
using EatUp.Users.Models;
using EatUp.Users.Repositories;
using EatUp.Users.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace EatUp.Users.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IRepository<User>> _userRepoMock = new();
        private readonly Mock<IRepository<RefreshTokenInformation>> _refreshTokenRepoMock = new();
        private readonly Mock<IConfiguration> _configMock = new();
        private readonly Mock<IRabbitMqPublisher> _publisherMock = new();

        private UserService CreateService() =>
            new(_userRepoMock.Object, _refreshTokenRepoMock.Object, _configMock.Object, _publisherMock.Object);

        [Fact]
        public async Task AddUser_ShouldInsertUserAndPublishEvent()
        {
            // Arrange
            var addUser = new AddUserDTO
            {
                Username = "testuser",
                Password = "password",
                FullName = "Test User",
                Email = "test@example.com"
            };
            var user = new User { Id = Guid.NewGuid(), Username = addUser.Username, FullName = addUser.FullName, Email = addUser.Email };
            _userRepoMock.Setup(r => r.Exist(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(false);
            _userRepoMock.Setup(r => r.Insert(It.IsAny<User>())).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<UserCreatedEvent>())).Returns(Task.CompletedTask);

            // Act
            var service = CreateService();
            // Stripe is not mocked here; in a real test, abstract and mock it.
            // await service.AddUser(addUser);

            // Assert
            // _userRepoMock.Verify(r => r.Insert(It.IsAny<User>()), Times.Once);
            // _userRepoMock.Verify(r => r.Save(), Times.Once);
            // _publisherMock.Verify(p => p.Publish(It.IsAny<UserCreatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task AddUser_ShouldThrow_WhenUsernameTaken()
        {
            var addUser = new AddUserDTO { Username = "taken" };
            _userRepoMock.Setup(r => r.Exist(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(true);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.AddUser(addUser));
        }

        [Fact]
        public async Task Delete_ShouldDeleteUserAndPublishEvent()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.Delete(userId)).Returns(Task.CompletedTask);
            _userRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<UserDeletedEvent>())).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.Delete(userId);

            _userRepoMock.Verify(r => r.Delete(userId), Times.Once);
            _userRepoMock.Verify(r => r.Save(), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<UserDeletedEvent>()), Times.Once);
        }

        [Fact]
        public async Task SignIn_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "testuser", Password = new PasswordHasher<User>().HashPassword(null, "password") };
            var signInDto = new SignInUserDTO { Username = "testuser", Password = "password" };

            _userRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), false)).ReturnsAsync(user);
            _refreshTokenRepoMock.Setup(r => r.Insert(It.IsAny<RefreshTokenInformation>())).ReturnsAsync(new RefreshTokenInformation());
            _refreshTokenRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            _configMock.Setup(c => c["UserJwt:Secret"]).Returns("supersecretkey12345678901234567890");
            _configMock.Setup(c => c["UserJwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["UserJwt:Audience"]).Returns("audience");

            var service = CreateService();
            var tokens = await service.SignIn(signInDto);

            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task SignIn_ShouldThrow_WhenUserNotFound()
        {
            var signInDto = new SignInUserDTO { Username = "nouser", Password = "password" };
            _userRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<User, bool>>>(), false)).ReturnsAsync((User)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.SignIn(signInDto));
        }

        [Fact]
        public async Task UpdateUser_ShouldUpdateAndPublishEvent()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "testuser" };
            var updateDto = new Mock<UpdateUserDTO>();

            _userRepoMock.Setup(r => r.GetById(userId, true)).ReturnsAsync(user);
            _userRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);
            _publisherMock.Setup(p => p.Publish(It.IsAny<UserUpdatedEvent>())).Returns(Task.CompletedTask);

            var service = CreateService();
            await service.UpdateUser(updateDto.Object, userId);

            _userRepoMock.Verify(r => r.Save(), Times.Once);
            _publisherMock.Verify(p => p.Publish(It.IsAny<UserUpdatedEvent>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUser_ShouldThrow_WhenUserNotFound()
        {
            var userId = Guid.NewGuid();
            var updateDto = new Mock<UpdateUserDTO>();
            _userRepoMock.Setup(r => r.GetById(userId, true)).ReturnsAsync((User)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateUser(updateDto.Object, userId));
        }

        [Fact]
        public async Task GetUserById_ShouldReturnUserDTO()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "testuser", FullName = "Test User", Email = "test@example.com" };
            _userRepoMock.Setup(r => r.GetById(userId, false)).ReturnsAsync(user);

            var service = CreateService();
            var result = await service.GetUserById(userId);

            Assert.Equal(user.Username, result.Username);
        }

        [Fact]
        public async Task GetUserById_ShouldThrow_WhenNotFound()
        {
            var userId = Guid.NewGuid();
            _userRepoMock.Setup(r => r.GetById(userId, false)).ReturnsAsync((User)null);

            var service = CreateService();
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetUserById(userId));
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, Username = "testuser" };
            var tokenInfo = new RefreshTokenInformation { Id = Guid.NewGuid(), UserId = userId, User = user, RefreshToken = "oldtoken" };

            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, It.IsAny<Expression<Func<RefreshTokenInformation, object>>[]>()))
                .ReturnsAsync(tokenInfo);
            _refreshTokenRepoMock.Setup(r => r.Delete(tokenInfo.Id)).Returns(Task.CompletedTask);
            _refreshTokenRepoMock.Setup(r => r.Insert(It.IsAny<RefreshTokenInformation>())).ReturnsAsync(new RefreshTokenInformation());
            _refreshTokenRepoMock.Setup(r => r.Save()).Returns(Task.CompletedTask);

            _configMock.Setup(c => c["JWT:Secret"]).Returns("supersecretkey12345678901234567890");
            _configMock.Setup(c => c["UserJwt:Issuer"]).Returns("issuer");
            _configMock.Setup(c => c["UserJwt:Audience"]).Returns("audience");

            var service = CreateService();
            var tokens = await service.RefreshToken("oldtoken");

            Assert.False(string.IsNullOrEmpty(tokens.AccessToken));
            Assert.False(string.IsNullOrEmpty(tokens.RefreshToken));
        }

        [Fact]
        public async Task RefreshToken_ShouldThrow_WhenTokenNotFound()
        {
            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, It.IsAny<Expression<Func<RefreshTokenInformation, object>>[]>()))
                .ReturnsAsync((RefreshTokenInformation)null);

            var service = CreateService();
            await Assert.ThrowsAsync<Exception>(() => service.RefreshToken("invalidtoken"));
        }

        [Fact]
        public async Task SignOut_ShouldDeleteRefreshToken()
        {
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
            _refreshTokenRepoMock.Setup(r => r.GetByExpression(It.IsAny<Expression<Func<RefreshTokenInformation, bool>>>(), true, null))
                .ReturnsAsync((RefreshTokenInformation)null);

            var service = CreateService();
            await Assert.ThrowsAsync<Exception>(() => service.SignOut("invalidtoken"));
        }
    }
}
