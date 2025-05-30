﻿using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EatUp.RabbitMQ.Events;
using EatUp.RabbitMQ.Events.Vendor;
using EatUp.Vendors.DTO;
using EatUp.Vendors.Extensions;
using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Geometries;
using Stripe;

namespace EatUp.Vendors.Services
{
    public class Vendorservice(IRabbitMqPublisher publisher, IRepository<Vendor> vendorRepository, IRepository<RefreshTokenInformation> refreshTokenRepository, IConfiguration configuration) : IVendorservice
    {
        public async Task<AccountLink> AddVendor(AddVendorDTO addVendor)
        {
            if (await UsernameIsTaken(addVendor.Username))
                throw new ArgumentException($"{nameof(addVendor.Username)} is already taken");

            string stripeAccountId = await CreateStripeAccount(addVendor);
            AccountLink? accountLink = await CreateAccountLink(stripeAccountId);

            Vendor vendor = addVendor.ToVendor();
            vendor.StripeAccountId = stripeAccountId;

            await vendorRepository.Insert(vendor);
            await vendorRepository.Save();

            var @event = ToCreatedEvent(stripeAccountId, vendor);
            await publisher.Publish(@event);

            return accountLink;
        }

        private static VendorCreatedEvent ToCreatedEvent(string stripeAccountId, Vendor vendor)
        {
            return new VendorCreatedEvent
            {
                Id = vendor.Id,
                Name = vendor.Name,
                StripeAccountId = stripeAccountId,
                Email = vendor.Email,
                Cvr = vendor.Cvr,
                Longitude = vendor.Location.X,
                Latitude = vendor.Location.Y
            };
        }

        private async Task<string> CreateStripeAccount(AddVendorDTO addVendor)
        {
            var options = new AccountCreateOptions
            {
                Country = "DK",
                Email = addVendor.Email,
                Controller = new AccountControllerOptions
                {
                    Fees = new AccountControllerFeesOptions { Payer = "application" },
                    Losses = new AccountControllerLossesOptions { Payments = "application" },
                    StripeDashboard = new AccountControllerStripeDashboardOptions
                    {
                        Type = "express",
                    },
                },
            };
            var service = new AccountService();
            var account = await service.CreateAsync(options);
            return account.Id;
        }

        private async Task<AccountLink> CreateAccountLink(string accountId)
        {
            var options = new AccountLinkCreateOptions
            {
                Account = accountId,
                RefreshUrl = configuration["StripeSettings:ReAuth"],
                ReturnUrl = configuration["StripeSettings:ReturnUrl"],
                Type = "account_onboarding",
            };
            var service = new AccountLinkService();
            var accountLink = await service.CreateAsync(options);
            return accountLink;
        }

        private async Task<bool> UsernameIsTaken(string username)
        {
            return await vendorRepository.Exist(x => x.Username == username);
        }

        public async Task<PaginationResult<VendorDTO>> GetPage(VendorSearchParams @params)
        {
            var userLocation = new Point(@params.Longitude, @params.Latitude) { SRID = 4326 };
            
            var expressions = new List<Expression<Func<Vendor, bool>>>();
            expressions.Add(vendor => vendor.Location != null && vendor.Location.Distance(userLocation) <= @params.Radius);
            
            if (!string.IsNullOrEmpty(@params.Search))
            {
                expressions.Add(vendor => vendor.Name.Contains(@params.Search));
            }

            return await vendorRepository.GetPage(@params.Skip, @params.Take, VendorDTO.FromVendor, expressions.AndAll());
        }

        public async Task Delete(Guid id)
        {
            await vendorRepository.Delete(id);
            await vendorRepository.Save();
            await publisher.Publish(new VendorDeletedEvent(id));
        }

        public async Task<VendorTokens> SignIn(SignInVendorDTO signInVendor)
        {
            var user = await vendorRepository.GetByExpression(x => x.Username == signInVendor.Username);
            if (user == null)
                throw new ArgumentException("User not found");

            if (!(await PasswordIsValid(user, signInVendor.Password)))
            {
                throw new ArgumentException("User not found");
            }

            var accessToken = GenerateAccessToken(user.Id.ToString(), user.Username, configuration["VendorJwt:Secret"], configuration["VendorJwt:Issuer"], configuration["VendorJwt:Audience"]);
            var refreshToken = GenerateRefreshToken();
            await SaveRefreshToken(user.Id, user.Username, refreshToken, accessToken);

            await refreshTokenRepository.Save();
            return new VendorTokens
            {
                RefreshToken = refreshToken,
                AccessToken = accessToken,
            };
        }

        private async Task<bool> PasswordIsValid(Vendor user, string password)
        {
            var passwordHasher = new PasswordHasher<Vendor>();
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
                    new Claim(ClaimTypes.Role, "Vendor"),
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
                Issuer = issuer,
                Audience = audience,
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task UpdateVendor(UpdateVendorDTO vendorDTO, Guid vendorId)
        {
            var vendorFromDb = await vendorRepository.GetById(vendorId, x => x, true);
            if (vendorFromDb == null)
                throw new ArgumentException("Vendor not found");

            vendorDTO.Merge(vendorFromDb);
            await vendorRepository.Save();
            await publisher.Publish(vendorDTO.ToEvent(vendorId));
        }

        public async Task<VendorDTO> GetVendorById(Guid vendorId)
        {
            var vendor = await vendorRepository.GetById(vendorId, VendorDTO.FromVendor);
            if (vendor == null)
                throw new ArgumentException("Vendor not found");

            return vendor;
        }

        public async Task<VendorTokens> RefreshToken(string refreshToken)
        {
            var tokenFromDb = await refreshTokenRepository.GetByExpression(x => x.RefreshToken == refreshToken, true, x => x.Vendor);
            if (tokenFromDb == null)
            {
                throw new Exception("refreshtoken is invalid");
            }

            await refreshTokenRepository.Delete(tokenFromDb.Id);

            var newRefreshToken = GenerateRefreshToken();
            var accessToken = GenerateAccessToken(tokenFromDb.VendorId.ToString(), tokenFromDb.Vendor.Username, configuration["VendorJwt:Secret"], configuration["VendorJwt:Issuer"], configuration["VendorJwt:Audience"]);

            await SaveRefreshToken(tokenFromDb.VendorId, tokenFromDb.Vendor.Username, newRefreshToken, accessToken);

            await refreshTokenRepository.Save();
            return new VendorTokens
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
            };
        }

        private async Task SaveRefreshToken(Guid vendorId, string username, string refreshToken, string accessToken)
        {
            var token = new RefreshTokenInformation
            {
                ExpirationDate = DateTime.UtcNow.AddDays(30),
                RefreshToken = refreshToken,
                VendorId = vendorId,
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
