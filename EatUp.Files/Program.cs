using System.Text;
using EatUp.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddTransient<IFileService>(x => new FileService(builder.Configuration["Azure:ConnectionString"]));

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



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
