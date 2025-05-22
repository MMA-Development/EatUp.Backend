using System.IdentityModel.Tokens.Jwt;
using System.Numerics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Users;
using EatUp.Users.DTO;
using EatUp.Users.Models;
using EatUp.Users.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Stripe;

namespace EatUp.Users.Services
{
    public class UserService(
        IRepository<User> userRepository, 
        IRepository<RefreshTokenInformation> refreshTokenRepository, 
        IConfiguration configuration, 
        IRabbitMqPublisher publisher) : IUserService
    {
        public async Task AddUser(AddUserDTO adduser)
        {
            if (await UsernameIsTaken(adduser.Username))
                throw new ArgumentException($"{nameof(adduser.Username)} is already taken");

            User user = adduser.ToUser();
            var stripeCustomerId = await CreateCustomerInStripe(user);
            user.StripeCustomerId = stripeCustomerId;
            await userRepository.Insert(user);
            await userRepository.Save();

            var @event = ToCreateEvent(user);
            await publisher.Publish(@event);
        }

        private UserCreatedEvent ToCreateEvent(User user) => new()
        {
            Id = user.Id,
            Fullname = user.Username,
            Email = user.Email,
            StripeCustomerId = user.StripeCustomerId,
        };

        private UserUpdatedEvent ToUpdatedEvent(User user) => new()
        {
            Id = user.Id,
            Fullname = user.Username,
            Email = user.Email,
            StripeCustomerId = user.StripeCustomerId,
        };

        private async Task<string> CreateCustomerInStripe(User user)
        {
            var options = new CustomerCreateOptions
            {
                Name = user.FullName,
                Email = user.Email,
            };
            var service = new CustomerService();
            Customer customer = await service.CreateAsync(options);
            return customer.Id;
        }

        private async Task<bool> UsernameIsTaken(string username)
        {
            return await userRepository.Exist(x => x.Username == username);
        }

        public async Task Delete(Guid id)
        {
            await userRepository.Delete(id);
            await userRepository.Save();
            var @event = new UserDeletedEvent(id);
            await publisher.Publish(@event);
        }

        public async Task<UserTokens> SignIn(SignInUserDTO singInUser)
        {
            var user = await userRepository.GetByExpression(x => x.Username == singInUser.Username);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!(await PasswordIsValid(user, singInUser.Password)))
            {
                throw new ArgumentException("User not found");
            }

            var accessToken = GenerateAccessToken(user.Id.ToString(), user.Username, configuration["UserJwt:Secret"], configuration["UserJwt:Issuer"], configuration["UserJwt:Audience"]);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, user.Username, refreshToken);

            await refreshTokenRepository.Save();
            return new UserTokens
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
            };
        }

        private async Task<bool> PasswordIsValid(User user, string password)
        {
            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }

        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            return Convert.ToBase64String(randomBytes);
        }

        private string GenerateAccessToken(string userId, string username, string secretKey, string issuer, string audience)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "User"),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task UpdateUser(UpdateUserDTO userDTO, Guid userId)
        {
            var userFromDb = await userRepository.GetById(userId, true);
            if (userFromDb == null)
                throw new ArgumentException("User not found");

            userDTO.Merge(userFromDb);
            await userRepository.Save();

            var @event = ToUpdatedEvent(userFromDb);
            await publisher.Publish(@event);
        }

        public async Task<UserDTO> GetUserById(Guid userId)
        {
            var user = await userRepository.GetById(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            return UserDTO.FromUser(user);
        }

        public async Task<UserTokens> RefreshToken(string refreshToken)
        {
            var tokenFromDb = await refreshTokenRepository.GetByExpression(x => x.RefreshToken == refreshToken, true, x => x.User);
            if (tokenFromDb == null)
            {
                throw new Exception("refreshtoken is invalid");
            }

            await refreshTokenRepository.Delete(tokenFromDb.Id);

            var newRefreshToken = GenerateRefreshToken();
            var accessToken = GenerateAccessToken(tokenFromDb.UserId.ToString(), tokenFromDb.User.Username, configuration["JWT:Secret"], configuration["UserJwt:Issuer"], configuration["UserJwt:Audience"]);

            await SaveRefreshToken(tokenFromDb.UserId, tokenFromDb.User.Username, newRefreshToken);

            await refreshTokenRepository.Save();
            return new UserTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
            };
        }

        private async Task SaveRefreshToken(Guid userId, string username, string refreshToken)
        {
            var token = new RefreshTokenInformation
            {
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                RefreshToken = refreshToken,
                UserId = userId,
            };
            await refreshTokenRepository.Insert(token);
        }

        public async Task SignOut(string refreshToken)
        {
            var tokenFromDb = await refreshTokenRepository.GetByExpression(x => x.RefreshToken == refreshToken, true);
            if (tokenFromDb == null)
            {
                throw new Exception("refreshtoken is invalid");
            }
            await refreshTokenRepository.Delete(tokenFromDb.Id);
            await refreshTokenRepository.Save();
        }
    }
}
