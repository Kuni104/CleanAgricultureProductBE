using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Repositories;
using CleanAgricultureProductBE.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using CleanAgricultureProductBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

namespace CleanAgricultureProductBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddOpenApi();
          
            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            .UseSeeding((context, _) =>
            {
                // Seed initial data here if necessary
                if (!context.Set<Account>().Any())
                {
                    var hasher = new PasswordHasher<Account>();

                    var adminAccount = new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 1,
                        Email = "admin@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                        PhoneNumber = "0123456789"
                    };

                    adminAccount.PasswordHash = hasher.HashPassword(adminAccount, adminAccount.PasswordHash);

                    context.Set<Account>().Add(adminAccount);
                    context.SaveChanges();
                }
            })
            );

            // Dependency Injection
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            // JWT Authentication
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                        )
                    };
                });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            // IMPORTANT ORDER
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
