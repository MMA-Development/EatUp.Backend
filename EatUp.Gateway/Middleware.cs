using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;

namespace EatUp.Gateway
{
    public class Middleware
    {
        public static Func<HttpContext, Func<Task>, Task> PayloadTransformer =
            (context, next) =>
            {
                if (context.Request.Headers.TryGetValue("Authorization", out var tokens))
                {
                    var jwtToken = tokens.FirstOrDefault()?.Substring("Bearer ".Length).Trim();
                    if (jwtToken != null)
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var token = handler.ReadJwtToken(jwtToken);
                        var userId = token.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                        var role = token.Claims.FirstOrDefault(c => c.Type == "role")?.Value;
                        if (role == null || userId == null)
                        {
                            throw new Exception("Invalid token");
                        }

                        if (role == "Vendor")
                        {
                            context.Request.Headers["vendorId"] = userId;
                        }
                        else if (role == "User")
                        {
                            context.Request.Headers["userId"] = userId;
                        }
                    }
                }
                return next();
            };
    }
}
