using EatUp.Vendors.AppSettings;
using EatUp.Vendors.DTO;
using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;
using Microsoft.Extensions.Options;
using Stripe;

namespace EatUp.Vendors.Services
{
    public class Vendorservice(IRepository<Vendor> repository, IConfiguration configuration) : IVendorservice
    {

        public async Task<AccountLink> AddVendor(AddVendorDTO addVendor)
        {
            if (await UsernameIsTaken(addVendor.Username))
                throw new ArgumentException($"{nameof(addVendor.Username)} is already taken");

            string stripeAccountId = await CreateStripeAccount(addVendor);
            AccountLink? accountLink = await CreateAccountLink(stripeAccountId);

            Vendor vendor = addVendor.ToVendor();
            vendor.StripeAccountId = stripeAccountId;

            await repository.Insert(vendor);
            await repository.Save();

            return accountLink;
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
            return await repository.Exist(x => x.Username == username);
        }

        public async Task<PaginationResult<Vendor>> GetPage(int skip, int take)
        {
            return await repository.GetPage(skip, take, null, false);
        }

        public async Task Delete(Guid id)
        {
            await repository.Delete(id);
            await repository.Save();
        }
    }
}
