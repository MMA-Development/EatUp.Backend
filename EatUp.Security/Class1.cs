using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace EatUp.Security
{
    public class SecurityPolicy
    {
        public static Action<JwtBearerOptions> ConfigureVendorPolicy(IConfiguration configuration)
        {
            return options =>
            {
                string secretKey = configuration["VendorJwt:Secret"] ?? throw new NullReferenceException("VendorJwt:Secret is null");
                string validIssuer = configuration["VendorJwt:Issuer"] ?? throw new NullReferenceException("VendorJwt:Issuer is null");
                string validAudience = configuration["VendorJwt:Audience"] ?? throw new NullReferenceException("VendorJwt:Audience is null");

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer,
                    ValidateAudience = true,
                    ValidAudience = validAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };
            };
        }

        public static Action<JwtBearerOptions> ConfigureUserPolicy(IConfiguration configuration)
        {
            return options =>
            {
                string secretKey = configuration["UserJwt:Secret"] ?? throw new NullReferenceException("UserJwt:Secret is null");
                string validIssuer = configuration["UserJwt:Issuer"] ?? throw new NullReferenceException("UserJwt:Issuer is null");
                string validAudience = configuration["UserJwt:Audience"] ?? throw new NullReferenceException("UserJwt:Audience is null");

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = validIssuer,
                    ValidateAudience = true,
                    ValidAudience = validAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                };
            };
        }
    }
}
