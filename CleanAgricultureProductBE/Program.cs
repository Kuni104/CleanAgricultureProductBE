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
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using CleanAgricultureProductBE.Repositories.DeliveryFee;
using CleanAgricultureProductBE.Services.DeliveryFee;
using CleanAgricultureProductBE.Repositories.PaymentMethod;
using CleanAgricultureProductBE.Services.PaymentMethod;
using CleanAgricultureProductBE.Services.Order;
using CleanAgricultureProductBE.Repositories.Order;
using CleanAgricultureProductBE.Services.Payment;
using CleanAgricultureProductBE.Repositories.Payment;
using CleanAgricultureProductBE.Services.OrderDetail;
using CleanAgricultureProductBE.Repositories.OrderDetail;


namespace CleanAgricultureProductBE
{
    public class Program
    {
        public static async Task Main(string[] args)
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

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Dependency Injection
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            // register token blacklist repo
            builder.Services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

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

            //Delivery Fee DI
            builder.Services.AddScoped<IDeliveryFeeService, DeliveryFeeService>();
            builder.Services.AddScoped<IDeliveryFeeRepository, DeliveryFeeRepository>();

            //PaymentMethod DI
            builder.Services.AddScoped<IPaymentMethodRepository, PaymentMethodRepository>();
            builder.Services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            
            //Order DI
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            //Order Detaill DI
            builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();

            //Payment DI
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

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

                    // Reject blacklisted tokens on validation
                    // Comment from here for temporary fix the JWT error
                    //options.Events = new JwtBearerEvents
                    //{
                    //   OnTokenValidated = async context =>
                    //   {
                    //       var token = context.SecurityToken as JwtSecurityToken;
                    //       if (token == null)
                    //       {
                    //           context.Fail("Invalid token");
                    //           return;
                    //       }

                    //       var tokenString = token.RawData;
                    //       // Resolve repository from DI
                    //       var repo = context.HttpContext.RequestServices.GetService<ITokenBlacklistRepository>();
                    //       if (repo != null)
                    //       {
                    //           var isBlacklisted = await repo.IsBlacklistedAsync(tokenString);
                    //           if (isBlacklisted)
                    //           {
                    //               context.Fail("Token revoked");
                    //           }
                    //       }
                    //   }
                    //};
                    // Comment End Here
                });

            // Thêm CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", builder =>
                {
                    builder.WithOrigins("http://localhost:5173") // Port của React app
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });


            builder.Services.AddAuthorization();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await DatabaseSeeder.SeedAsync(dbContext);
            };

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
