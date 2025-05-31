using System.Text;
using EatUp.Meals;
using EatUp.Meals.EventHandlers;
using EatUp.Meals.Models;
using EatUp.Meals.Repositories;
using EatUp.Meals.Services;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Meals;
using EatUp.RabbitMQ.Events.Order;
using EatUp.RabbitMQ.Events.Vendor;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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
    });

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
    options.AddPolicy("default", policy =>
    {
        policy.AddAuthenticationSchemes("UserScheme", "PolicyScheme");
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
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));

//repositrories
builder.Services.AddTransient<IRepository<Meal>, Repository<Meal>>();
builder.Services.AddTransient<IRepository<Review>, Repository<Review>>();
builder.Services.AddTransient<IRepository<Category>, Repository<Category>>();
builder.Services.AddTransient<IRepository<VendorProjection>, Repository<VendorProjection>>();
builder.Services.AddTransient<IRepository<CompletedOrderProjection>, Repository<CompletedOrderProjection>>();
builder.Services.AddTransient<IRecommendationRepository, RecommendationRepository>();

//services
builder.Services.AddTransient<IMealService, MealService>();
builder.Services.AddTransient<ICategoryService, CategoryService>();
builder.Services.AddTransient<IRecommendationService, RecommendationService>();

//Event handlers
builder.Services.AddSingleton<EventDispatcher>();
builder.Services.AddTransient<IEventHandler<VendorCreatedEvent>, VendorCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<VendorUpdatedEvent>, VendorUpdatedEventHandler>();
builder.Services.AddTransient<IEventHandler<VendorDeletedEvent>, VendorDeletedEventHandler>();
builder.Services.AddTransient<IEventHandler<VendorHardResyncEvent>, VendorHardResyncEventHandler>();

builder.Services.AddTransient<IEventHandler<OrderCompletedEvent>, OrderCompletedEventHandler>();

builder.Services.AddTransient<IEventHandler<PerformMealHardResyncEvent>, PerformMealHardResyncEventHandler>();

builder.Services.AddSingleton<IRabbitMqPublisher>(x =>
    new RabbitMqPublisher(
        builder.Configuration["RabbitMQ:Host"],
        "events",
        builder.Configuration["RabbitMQ:Username"],
        builder.Configuration["RabbitMQ:Password"]
    ));

builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddSeq(serverUrl: builder.Configuration["Seq:ServerUrl"], apiKey: builder.Configuration["Seq:ApiKey"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Context>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    dbContext.Database.Migrate();

    var dispatcher = scope.ServiceProvider.GetRequiredService<EventDispatcher>();
    var consumer = new RabbitMqConsumer(builder.Configuration["RabbitMQ:Host"], "events", "meals", builder.Configuration["RabbitMQ:Username"], builder.Configuration["RabbitMQ:Password"], dispatcher, logger);
    await consumer.Start();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
