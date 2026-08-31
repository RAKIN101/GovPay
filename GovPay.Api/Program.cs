using GovPay.Application.Services;
using GovPay.Application.Interfaces;
using GovPay.Application.Configuration;
using GovPay.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using GovPay.Infrastructure.Data;
using GovPay.Infrastructure.Repositories;
using GovPay.Cryptography.Hashing;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
public partial class Program
{
    private static void Main(string[] args)
    {   
        var builder = WebApplication.CreateBuilder(args);
        var jwtSettings = new JwtSettings();

        builder.Configuration
            .GetSection("Jwt")
            .Bind(jwtSettings);

        builder.Services.AddSingleton(jwtSettings);
        builder.Services.AddDbContext<GovPayDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("GovPayDatabase")));
        // Add services to the container.
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:3000", "http://127.0.0.1:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddScoped<UserService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<PasswordHasher>();
        builder.Services.AddScoped<TwoFactorService>();
        builder.Services.AddScoped<IJwtService, JwtService>();
        builder.Services.AddScoped<IBillRepository, BillRepository>();
        builder.Services.AddScoped<BillService>();
        builder.Services.AddScoped<BillStatusService>();
        builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
        builder.Services.AddScoped<IPaymentService, PaymentService>();
        builder.Services.AddScoped<IPostRepository, PostRepository>();
        builder.Services.AddScoped<PostService>();
        builder.Services.AddScoped<UserProfileService>();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<GovPayDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<PasswordHasher>();

            var databaseExists = dbContext.Database.CanConnect();
            if (!databaseExists)
            {
                dbContext.Database.EnsureCreated();
            }

            var adminUser = dbContext.Users
                .Where(u => u.Username == "Salman2")
                .OrderByDescending(u => u.Id)
                .FirstOrDefault();

            if (adminUser is null)
            {
                var newAdmin = new User
                {
                    Username = "Salman2",
                    Email = "salman2@govpay.com",
                    Role = "Admin",
                    TwoFactorEnabled = false,
                };

                var (hash, salt) = passwordHasher.HashPassword("Password123!");
                newAdmin.PasswordHash = hash;
                newAdmin.PasswordSalt = salt;

                dbContext.Users.Add(newAdmin);
            }
            else
            {
                var (hash, salt) = passwordHasher.HashPassword("Password123!");
                adminUser.Email = "salman2@govpay.com";
                adminUser.Role = "Admin";
                adminUser.PasswordHash = hash;
                adminUser.PasswordSalt = salt;
                adminUser.TwoFactorEnabled = false;
                adminUser.TwoFactorCodeHash = null;
                adminUser.TwoFactorCodeSalt = null;
                adminUser.TwoFactorCodeExpiresAt = null;
            }

            var duplicateUsers = dbContext.Users
                .Where(u => u.Username == "Salman2")
                .OrderBy(u => u.Id)
                .Skip(1)
                .ToList();

            if (duplicateUsers.Count > 0)
            {
                dbContext.Users.RemoveRange(duplicateUsers);
            }

            if (!dbContext.Users.Any(u => u.Username == "citizen1"))
            {
                var citizen = new User
                {
                    Username = "citizen1",
                    Email = "citizen1@govpay.local",
                    Role = "Citizen",
                    TwoFactorEnabled = false,
                };

                var (citizenHash, citizenSalt) = passwordHasher.HashPassword("Password123!");
                citizen.PasswordHash = citizenHash;
                citizen.PasswordSalt = citizenSalt;

                dbContext.Users.Add(citizen);
            }

            dbContext.SaveChanges();
        }

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseCors("AllowFrontend");
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        var summaries = new[]
        {
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast");

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
