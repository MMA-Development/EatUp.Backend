using System.IdentityModel.Tokens.Jwt;
using System.Text;
using EatUp.Gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(serverUrl: builder.Configuration["Seq:ServerUrl"], apiKey: builder.Configuration["Seq:ApiKey"]);
});

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/vendors/v1/swagger.json", "EatUp.Vendors");
        options.SwaggerEndpoint("/swagger/users/v1/swagger.json", "EatUp.Users"); 
        options.SwaggerEndpoint("/swagger/meals/v1/swagger.json", "EatUp.Meals");
        options.SwaggerEndpoint("/swagger/orders/v1/swagger.json", "EatUp.Orders");

        options.RoutePrefix = "swagger";
    });
}

app.Use(Middleware.Logging);

app.UseHttpsRedirection();

app.MapReverseProxy(proxyPipeline =>
{
    proxyPipeline.Use(Middleware.PayloadTransformer);
});

app.UseCors((x) => x.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());

app.Run();