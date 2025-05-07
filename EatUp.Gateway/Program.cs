using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));


var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use((context, next) =>
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
    });
});

app.UseCors((x) => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
