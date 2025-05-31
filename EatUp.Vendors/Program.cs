using System.Text;
using EatUp.RabbitMQ;
using EatUp.RabbitMQ.Events.Vendor;
using EatUp.Vendors;
using EatUp.Vendors.EventHandlers;
using EatUp.Vendors.Models;
using EatUp.Vendors.Repositories;
using EatUp.Vendors.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), x =>
    {
        x.UseNetTopologySuite();
    }));
builder.Services.AddTransient<IRepository<Vendor>, Repository<Vendor>>();
builder.Services.AddTransient<IRepository<RefreshTokenInformation>, Repository<RefreshTokenInformation>>();
builder.Services.AddTransient<IVendorservice, Vendorservice>();
builder.Services.AddTransient<IEventHandler<PerformVendorHardResyncEvent>, PerformVendorHardResyncEventHandler>();
builder.Services.AddSingleton<IRabbitMqPublisher>(x => 
    new RabbitMqPublisher(
        builder.Configuration["RabbitMQ:Host"], 
        "events", 
        builder.Configuration["RabbitMQ:Username"], 
        builder.Configuration["RabbitMQ:Password"]
    ));
builder.Services.AddSingleton<EventDispatcher>();

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
    dbContext.Database.Migrate();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    var dispatcher = scope.ServiceProvider.GetRequiredService<EventDispatcher>();
    var consumer = new RabbitMqConsumer(builder.Configuration["RabbitMQ:Host"], "events", "vendors", builder.Configuration["RabbitMQ:Username"], builder.Configuration["RabbitMQ:Password"], dispatcher, logger);
    await consumer.Start();
}

StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:Secret"];

app.UseAuthorization();
app.MapControllers();

app.Run();
