using EatUp.Hangfire.Jobs;
using Hangfire;
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


// Add services to the container.
builder.Services.AddHangfire(configuration => configuration.SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection")));

builder.Services.AddHangfireServer();

var app = builder.Build();


app.UseHangfireDashboard();

ConfigureJobs.Configure(app);

app.Run();

