using CleanAgricultureProductBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            await context.Database.MigrateAsync();

            //Account seeding
            if (!await context.Set<Account>().AnyAsync())
            {
                var hasher = new PasswordHasher<Account>();

                var accountList = new List<Account>
                {
                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 1,
                        Email = "admin@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                        PhoneNumber = "0123456789"
                    },

                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 2,
                        Email = "user@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                    },

                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 3,
                        Email = "staff@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                    },

                    new Account
                    {
                        AccountId = Guid.NewGuid(),
                        RoleId = 4,
                        Email = "delivery@gmail.com",
                        PasswordHash = "12345",
                        Status = "Active",
                    }
                };

                foreach (var account in accountList)
                {
                    account.PasswordHash = hasher.HashPassword(account, account.PasswordHash);
                }

                context.Accounts.AddRange(accountList);
                await context.SaveChangesAsync();

            }

            //UserProfile seeding
            if (!await context.Set<UserProfile>().AnyAsync())
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
                await context.SaveChangesAsync();
            }

            //Address Seeding
            if (!await context.Set<Address>().AnyAsync())
            {
                var useraddress = new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserProfileId = context.UserProfiles.Include(uf => uf.Account)
                                                        .Where(a => a.Account.Email == "user@gmail.com")
                                                        .Select(a => a.UserProfileId)
                                                        .FirstOrDefault(),
                    RecipientName = "John Doe",
                    RecipientPhone = "1023456789",
                    Ward = "A",
                    District = "A",
                    City = "A",
                    AddressDetail = "AAA",
                    IsDefault = true
                };

                context.Addresses.Add(useraddress);
                await context.SaveChangesAsync();
            }

            //Cart seeding
            if (!await context.Set<Cart>().AnyAsync())
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
                await context.SaveChangesAsync();
            }

            //Category seeding
            if (!await context.Set<Category>().AnyAsync())
            {
                var categories = new List<Category>
                {
                    new Category { CategoryId = Guid.NewGuid(), Name = "Fruits", Description = "Fresh fruits", Status = "Active" },
                    new Category { CategoryId = Guid.NewGuid(), Name = "Vegetables", Description = "Fresh vegetables", Status = "Active" },
                    new Category { CategoryId = Guid.NewGuid(), Name = "Grains", Description = "Healthy grains", Status = "Active" }
                };

                context.Set<Category>().AddRange(categories);
                await context.SaveChangesAsync();
            }

            //Product seeding
            if (!await context.Set<Product>().AnyAsync())
            {
                var products = new List<Product>
                {
                    new Product
                    {
                        ProductId = Guid.NewGuid(),
                        CategoryId = context.Set<Category>()
                                            .Where(c => c.Name == "Fruits")
                                            .Select(c => c.CategoryId)
                                            .FirstOrDefault(),
                        Name = "Apple",
                        Description = "Fresh red apples",
                        Price = 10000,
                        Unit = 1,
                        Stock = 100,
                        Status = "In Stock",
                    },
                    new Product
                    {
                        ProductId = Guid.NewGuid(),
                        CategoryId = context.Set<Category>()
                                            .Where(c => c.Name == "Fruits")
                                            .Select(c => c.CategoryId)
                                            .FirstOrDefault(),
                        Name = "Peach",
                        Description = "Fresh Peach Made In Heaven",
                        Price = 10400,
                        Unit = 1,
                        Stock = 104,
                        Status = "In Stock"
                    },
                    new Product
                    {
                        ProductId = Guid.NewGuid(),
                        CategoryId = context.Set<Category>()
                                            .Where(c => c.Name == "Vegetables")
                                            .Select(c => c.CategoryId)
                                            .FirstOrDefault(),
                        Name = "Carrot",
                        Description = "Organic carrots",
                        Price = 8000,
                        Unit = 1,
                        Stock = 150,
                        Status = "In Stock"
                    },
                    new Product
                    {
                        ProductId = Guid.NewGuid(),
                        CategoryId = context.Set<Category>()
                                            .Where(c => c.Name == "Grains")
                                            .Select(c => c.CategoryId)
                                            .FirstOrDefault(),
                        Name = "Rice",
                        Description = "Brown rice",
                        Price = 25000,
                        Unit = 1,
                        Stock = 200,
                        Status = "In Stock"
                    }
                };

                context.Set<Product>().AddRange(products);
                await context.SaveChangesAsync();
            }

            //PaymentMethod seeding
            if (!await context.Set<PaymentMethod>().AnyAsync())
            {
                var paymentMethods = new List<PaymentMethod>
                {
                    new PaymentMethod 
                    {
                        MethodName = "Cash On Delivery" 
                    }
                };

                context.Set<PaymentMethod>().AddRange(paymentMethods);
                await context.SaveChangesAsync();
            }

            //DeliveryFee Seeding
            if (!await context.Set<DeliveryFee>().AnyAsync())
            {
                var deliveryFees = new List<DeliveryFee>
                {
                    new DeliveryFee
                    {
                        DeliveryFeeId = Guid.NewGuid(),
                        District = "A",
                        City = "A",
                        FeeAmount = 25,
                        EffectiveDay = DateTime.UtcNow,
                        EstimatedDay = DateTime.UtcNow.AddYears(5)
                    }
                };

                context.DeliveryFees.AddRange(deliveryFees);
                await context.SaveChangesAsync();
            }

            //Schedule Seeding
            if(!await context.Set<Schedule>().AnyAsync())
            {
                var schedules = new List<Schedule>
                {
                    new Schedule
                    {
                        ScheduleId = Guid.NewGuid(),
                        DeliveryPersonId = context.Accounts.Where(a => a.Email == "delivery@gmail.com")
                                                           .Select(a => a.AccountId)
                                                           .FirstOrDefault(),
                        ScheduledDate = DateTime.UtcNow.AddDays(7),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = "Active"
                    }
                };

                context.Schedules.AddRange(schedules);
                await context.SaveChangesAsync();
            }
        }
    }
}
