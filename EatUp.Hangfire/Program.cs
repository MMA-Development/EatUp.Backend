using EatUp.Hangfire;
using EatUp.Hangfire.Jobs;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Dashboard.BasicAuthorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.AddConsole();
builder.Services.AddTransient<IRabbitMqPublisher>(x =>
    new RabbitMqPublisher(
        builder.Configuration["RabbitMQ:Host"],
        "events",
        builder.Configuration["RabbitMQ:Username"],
        builder.Configuration["RabbitMQ:Password"]
    ));


builder.Services.AddScoped<PerformVendorHardResyncJob>();
builder.Services.AddScoped<PerformUserHardResyncJob>();
builder.Services.AddScoped<PerformMealHardResyncJob>();

var options = new DashboardOptions
{
    Authorization = new[] { new BasicAuthAuthorizationFilter(
        new BasicAuthAuthorizationFilterOptions
        {
            RequireSsl = false, // true in production
            SslRedirect = false,
            LoginCaseSensitive = true,
            Users = new[]
            {
                new BasicAuthAuthorizationUser
                {
                    Login = builder.Configuration["Hangfire:Username"] ,
                    PasswordClear = builder.Configuration["Hangfire:Password"]
                }
            }
        })
    }
};

// Add services to the container.
builder.Services.AddHangfire(configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();

EnsureDatabaseCreated.Ensure(builder.Configuration.GetConnectionString("HangfireConnection"));

app.UseHangfireDashboard(options: options);

ConfigureJobs.Configure(app);

app.Run();

