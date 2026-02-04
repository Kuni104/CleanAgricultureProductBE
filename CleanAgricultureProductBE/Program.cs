
using CleanAgricultureProductBE.Data;
using CleanAgricultureProductBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

namespace CleanAgricultureProductBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
