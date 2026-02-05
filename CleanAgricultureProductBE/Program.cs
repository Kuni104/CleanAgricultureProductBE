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
using CleanAgricultureProductBE.Services.Cart;
using CleanAgricultureProductBE.Repositories.Cart;
using CleanAgricultureProductBE.Repositories.CartItem;

namespace CleanAgricultureProductBE
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    Description = "Nhập JWT token của bạn"
                });

                options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
                {
                    {
                        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                        {
                            Reference = new Microsoft.OpenApi.Models.OpenApiReference
                            {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });


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

                    var userAccount = new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 2,
                        Email = "user@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                        PhoneNumber = "0123456789"
                    };


                    adminAccount.PasswordHash = hasher.HashPassword(adminAccount, adminAccount.PasswordHash);
                    userAccount.PasswordHash = hasher.HashPassword(userAccount, userAccount.PasswordHash);

                    context.Set<Account>().Add(adminAccount);
                    context.Set<Account>().Add(userAccount);
                    context.SaveChanges();

                }

                if (!context.Set<UserProfile>().Any())
                {
                    var userProfile = new UserProfile
                    {
                        UserProfileId = Guid.NewGuid(),
                        AccountId = context.Set<Account>()
                                            .Where(a => a.Email == "user@gmail.com")
                                            .Select(a => a.AccountId)
                                            .FirstOrDefault(),
                        FirstName = "John",
                        LastName = "Doe"
                    };
                    context.Set<UserProfile>().Add(userProfile);
                    context.SaveChanges();
                }

                if (!context.Set<Cart>().Any())
                {

                    var account = context.Set<Account>()
                                    .Include(a => a.UserProfile)
                                    .Where(a => a.Email == "user@gmail.com")
                                    .FirstOrDefault();

                    var userCart = new Cart
                    {
                        CartId = Guid.NewGuid(),
                        CustomerId = account!.UserProfile.UserProfileId,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    context.Set<Cart>().Add(userCart);
                    context.SaveChanges();
                }
            })
            );

            // Dependency Injection
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();

            // Product DI
            builder.Services.AddScoped<IProductService, ProductService>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            // Category DI
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();

            //Cart DI
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<ICartRepository, CartRepository>();

            //Cart Item DI
            builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();

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

            // Th�m CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("http://localhost:5173") // Port c?a React app
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });


            builder.Services.AddAuthorization();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.MapScalarApiReference();
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");
            // IMPORTANT ORDER
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
