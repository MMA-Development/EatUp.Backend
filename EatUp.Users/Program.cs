using System.Text;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Order;
using EatUp.RabbitMQ.Events.Users;
using EatUp.Users;
using EatUp.Users.EventHandlers;
using EatUp.Users.Models;
using EatUp.Users.Repositories;
using EatUp.Users.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("VendorScheme", options =>
    {
        string secretKey = builder.Configuration["VendorJwt:Secret"] ?? throw new NullReferenceException("VendorJwt:Secret is null");
        string validIssuer = builder.Configuration["VendorJwt:Issuer"] ?? throw new NullReferenceException("VendorJwt:Issuer is null");
        string validAudience = builder.Configuration["VendorJwt:Audience"] ?? throw new NullReferenceException("VendorJwt:Audience is null");

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
    }).AddJwtBearer("UserScheme", options =>
    {
        string secretKey = builder.Configuration["UserJwt:Secret"] ?? throw new NullReferenceException("UserJwt:Secret is null");
        string validIssuer = builder.Configuration["UserJwt:Issuer"] ?? throw new NullReferenceException("UserJwt:Issuer is null");
        string validAudience = builder.Configuration["UserJwt:Audience"] ?? throw new NullReferenceException("UserJwt:Audience is null");

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
    }).AddJwtBearer();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Vendor", policy =>
    {
        policy.AuthenticationSchemes.Add("VendorScheme");
        policy.RequireAuthenticatedUser();
    });
    options.AddPolicy("User", policy =>
    {
        policy.AuthenticationSchemes.Add("UserScheme");
        policy.RequireAuthenticatedUser();
    });
});

builder.Services.Configure<AuthorizationOptions>(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder()
        .AddAuthenticationSchemes("VendorScheme", "UserScheme")
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(serverUrl: builder.Configuration["Seq:ServerUrl"], apiKey: builder.Configuration["Seq:ApiKey"]);
});
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddTransient<IRepository<User>, Repository<User>>();
builder.Services.AddTransient<IRepository<UserFavorite>, Repository<UserFavorite>>();
builder.Services.AddTransient<IRepository<OrderCompletedProjection>, Repository<OrderCompletedProjection>>();

builder.Services.AddTransient<IRepository<RefreshTokenInformation>, Repository<RefreshTokenInformation>>();
builder.Services.AddTransient<IUserService, UserService>();

//EventHandlers
builder.Services.AddSingleton<EventDispatcher>();
builder.Services.AddTransient<IEventHandler<PerformUserHardResyncEvent>, PerformUserHardResyncEventHandler>();
builder.Services.AddTransient<IEventHandler<PerformUserFavoriteHardResyncEvent>, PerformUserFavoriteHardResyncEventHandler>();
builder.Services.AddTransient<IEventHandler<OrderCompletedEvent>, OrderCompletedEventHandler>();

builder.Services.AddSingleton<IRabbitMqPublisher>(x =>
    new RabbitMqPublisher(
        builder.Configuration["RabbitMQ:Host"],
        "events",
        builder.Configuration["RabbitMQ:Username"],
        builder.Configuration["RabbitMQ:Password"]
    ));
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    dbContext.Database.Migrate();

    var dispatcher = scope.ServiceProvider.GetRequiredService<EventDispatcher>();
    var consumer = new RabbitMqConsumer(builder.Configuration["RabbitMQ:Host"], "events", "users", builder.Configuration["RabbitMQ:Username"], builder.Configuration["RabbitMQ:Password"], dispatcher, logger);
    await consumer.Start();
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    dbContext.Database.Migrate();
}
StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:Secret"] ?? throw new NullReferenceException("StripeSettings:Secret is null");

app.UseAuthorization();
app.MapControllers();

app.Run();
