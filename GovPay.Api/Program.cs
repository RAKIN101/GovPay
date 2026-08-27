using GovPay.Application.Services;
using GovPay.Application.Interfaces;
using GovPay.Application.Configuration;
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

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
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
