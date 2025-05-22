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

        public static Func<HttpContext, Func<Task>, Task> Logging =
            async (context, next) =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                // Log request
                context.Request.EnableBuffering();
                var requestBody = "";
                if (context.Request.ContentLength > 0)
                {
                    context.Request.Body.Position = 0;
                    using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                    requestBody = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;
                }
                logger.LogInformation("Request {Method} {Path} Body={Body}",
                    context.Request.Method, context.Request.Path, requestBody);

                // Capture response
                var originalBodyStream = context.Response.Body;
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await next();

                context.Response.Body.Seek(0, SeekOrigin.Begin);
                var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                logger.LogInformation("Response {StatusCode} {Path} Body={Body}",
                    context.Response.StatusCode, context.Request.Path, responseText);

                await responseBody.CopyToAsync(originalBodyStream);
            };
    }
}
