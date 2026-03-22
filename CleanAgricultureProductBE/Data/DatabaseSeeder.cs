using CleanAgricultureProductBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // Account + UserProfile seeding

            var hasher = new PasswordHasher<Account>();

            // ---- ACCOUNTS ----
            var accountList = new List<Account>
{
            // Admin
            new Account
            {
                AccountId = Guid.NewGuid(),
                RoleId = 1,
                Email = "admin@gmail.com",
                PasswordHash = "12345",
                Status = "Active",
                PhoneNumber = "0123456789"
            },

            // Customers
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user2@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user3@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user4@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user5@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user6@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user7@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user8@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user9@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user10@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 2, Email = "user11@gmail.com", PasswordHash = "12345", Status = "Active" },

            // Staff
            new Account { AccountId = Guid.NewGuid(), RoleId = 3, Email = "staff@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 3, Email = "staff2@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 3, Email = "staff3@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 3, Email = "staff4@gmail.com", PasswordHash = "12345", Status = "Active" },

            // Delivery
            new Account { AccountId = Guid.NewGuid(), RoleId = 4, Email = "delivery@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 4, Email = "delivery2@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 4, Email = "delivery3@gmail.com", PasswordHash = "12345", Status = "Active" },
            new Account { AccountId = Guid.NewGuid(), RoleId = 4, Email = "delivery4@gmail.com", PasswordHash = "12345", Status = "Active" },
            };

            // Hash passwords
            foreach (var acc in accountList)
            {
                acc.PasswordHash = hasher.HashPassword(acc, acc.PasswordHash);
            }

            // Get existing emails
            var existingEmails = await context.Set<Account>()
                .Select(a => a.Email)
                .ToListAsync();

            // Insert only new accounts
            var newAccounts = accountList
                .Where(a => !existingEmails.Contains(a.Email))
                .ToList();

            if (newAccounts.Any())
            {
                context.Accounts.AddRange(newAccounts);
                await context.SaveChangesAsync();
            }

            // ---- USER PROFILES ----

            // Always rebuild dictionary from DB (safe even if partial insert happened)
            var accountDict = await context.Set<Account>()
                .ToDictionaryAsync(a => a.Email, a => a.AccountId);

            var userProfileList = new List<UserProfile>
        {
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["admin@gmail.com"], FirstName = "John", LastName = "Admin" },

            // Customers
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user@gmail.com"], FirstName = "John", LastName = "Doe" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user2@gmail.com"], FirstName = "User", LastName = "2" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user3@gmail.com"], FirstName = "User", LastName = "3" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user4@gmail.com"], FirstName = "User", LastName = "4" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user5@gmail.com"], FirstName = "User", LastName = "5" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user6@gmail.com"], FirstName = "User", LastName = "6" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user7@gmail.com"], FirstName = "User", LastName = "7" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user8@gmail.com"], FirstName = "User", LastName = "8" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user9@gmail.com"], FirstName = "User", LastName = "9" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user10@gmail.com"], FirstName = "User", LastName = "10" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["user11@gmail.com"], FirstName = "User", LastName = "11" },

            // Staff
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["staff@gmail.com"], FirstName = "John", LastName = "Staff1" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["staff2@gmail.com"], FirstName = "John", LastName = "Staff2" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["staff3@gmail.com"], FirstName = "John", LastName = "Staff3" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["staff4@gmail.com"], FirstName = "John", LastName = "Staff4" },

            // Delivery
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["delivery@gmail.com"], FirstName = "John", LastName = "Delivery1" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["delivery2@gmail.com"], FirstName = "John", LastName = "Delivery2" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["delivery3@gmail.com"], FirstName = "John", LastName = "Delivery3" },
            new UserProfile { UserProfileId = Guid.NewGuid(), AccountId = accountDict["delivery4@gmail.com"], FirstName = "John", LastName = "Delivery4" },
};

            // Get existing AccountIds in UserProfile
            var existingProfileAccountIds = await context.Set<UserProfile>()
                .Select(up => up.AccountId)
                .ToListAsync();

            // Insert only profiles that don’t exist
            var newProfiles = userProfileList
                .Where(up => !existingProfileAccountIds.Contains(up.AccountId))
                .ToList();

            if (newProfiles.Any())
            {
                context.Set<UserProfile>().AddRange(newProfiles);
                await context.SaveChangesAsync();
            }

            // ---- ADDRESS SEEDING ----

            // Get all customer profiles (RoleId = 2)
            var customerProfiles = await context.UserProfiles
                .Include(up => up.Account)
                .Where(up => up.Account.RoleId == 2)
                .ToListAsync();

            // Get existing addresses (by UserProfileId)
            var existingAddressProfileIds = await context.Addresses
                .Select(a => a.UserProfileId)
                .ToListAsync();

            // Prepare new addresses (same info for all)
            var newAddresses = customerProfiles
                .Where(up => !existingAddressProfileIds.Contains(up.UserProfileId))
                .Select(up => new Address
                {
                    AddressId = Guid.NewGuid(),
                    UserProfileId = up.UserProfileId,
                    RecipientName = "John Doe",
                    RecipientPhone = "1023456789",
                    Ward = "Quận 8",
                    District = "Ba Đình",
                    City = "HCM",
                    AddressDetail = "123",
                    IsDefault = true
                })
                .ToList();

            // Insert only missing ones
            if (newAddresses.Any())
            {
                context.Addresses.AddRange(newAddresses);
                await context.SaveChangesAsync();
            }

            //CART SEEDING
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

            // ---- AGRICULTURE CATEGORY SEEDING ----

            var categories = new List<Category>
            {
            new Category { CategoryId = Guid.NewGuid(), Name = "Fruits", Description = "Fresh fruits from farms", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Vegetables", Description = "Fresh vegetables from farms", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Grains", Description = "Cereal crops like wheat, corn, rice", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Seeds", Description = "Seeds for planting crops", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Seedlings", Description = "Young plants ready for transplanting", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Fertilizers", Description = "Soil nutrients and fertilizers", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Pesticides", Description = "Crop protection chemicals", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Herbicides", Description = "Weed control products", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Animal Feed", Description = "Feed for livestock", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Livestock", Description = "Farm animals like cattle, pigs, poultry", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Dairy Products", Description = "Milk and dairy from farms", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Eggs", Description = "Fresh farm eggs", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Organic Produce", Description = "Certified organic agricultural products", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Aquaculture", Description = "Fish and aquatic farming products", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Farm Equipment", Description = "Tools and machinery for farming", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Irrigation", Description = "Water supply and irrigation systems", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Soil Amendments", Description = "Materials to improve soil quality", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Compost", Description = "Organic compost and waste recycling", Status = "Active" },

            new Category { CategoryId = Guid.NewGuid(), Name = "Spices and Herbs", Description = "Cultivated herbs and spices", Status = "Active" },
            new Category { CategoryId = Guid.NewGuid(), Name = "Plantation Crops", Description = "Coffee, tea, rubber and similar crops", Status = "Active" }
            };

            // Get existing category names
            var existingNames = await context.Set<Category>()
                .Select(c => c.Name)
                .ToListAsync();

            // Insert only missing ones
            var newCategories = categories
                .Where(c => !existingNames.Contains(c.Name))
                .ToList();

            if (newCategories.Any())
            {
                context.Set<Category>().AddRange(newCategories);
                await context.SaveChangesAsync();
            }

            // ---- PRODUCT SEEDING ----

            // Build category lookup once
            var categoryDict = await context.Set<Category>()
                .ToDictionaryAsync(c => c.Name, c => c.CategoryId);

            var products = new List<Product>
            {
            // Existing
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Fruits"], Name = "Apple", Description = "Fresh red apples", Price = 10000, Unit = "kg", Stock = 100, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Fruits"], Name = "Peach", Description = "Fresh peaches", Price = 10400, Unit = "kg", Stock = 104, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Vegetables"], Name = "Carrot", Description = "Organic carrots", Price = 8000, Unit = "kg", Stock = 150, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Grains"], Name = "Rice", Description = "Brown rice", Price = 25000, Unit = "kg", Stock = 200, Status = "Active" },

            // Additional 16 (1 product per category)
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Seeds"], Name = "Corn Seeds", Description = "High yield corn seeds", Price = 50000, Unit = "bag", Stock = 50, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Seedlings"], Name = "Tomato Seedlings", Description = "Healthy tomato plants", Price = 3000, Unit = "plant", Stock = 200, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Fertilizers"], Name = "NPK Fertilizer", Description = "Balanced fertilizer", Price = 200000, Unit = "bag", Stock = 80, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Pesticides"], Name = "Insecticide A", Description = "Protect crops from pests", Price = 120000, Unit = "bottle", Stock = 60, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Herbicides"], Name = "Weed Killer", Description = "Effective weed control", Price = 110000, Unit = "bottle", Stock = 70, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Animal Feed"], Name = "Chicken Feed", Description = "Nutritional poultry feed", Price = 180000, Unit = "bag", Stock = 90, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Livestock"], Name = "Young Pig", Description = "Healthy piglet", Price = 1500000, Unit = "unit", Stock = 20, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Dairy Products"], Name = "Fresh Milk", Description = "Raw cow milk", Price = 30000, Unit = "liter", Stock = 120, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Eggs"], Name = "Chicken Eggs", Description = "Farm fresh eggs", Price = 25000, Unit = "dozen", Stock = 200, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Organic Produce"], Name = "Organic Lettuce", Description = "Certified organic lettuce", Price = 15000, Unit = "kg", Stock = 100, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Aquaculture"], Name = "Tilapia Fish", Description = "Fresh farmed fish", Price = 60000, Unit = "kg", Stock = 70, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Farm Equipment"], Name = "Hand Tractor", Description = "Small farming tractor", Price = 15000000, Unit = "unit", Stock = 10, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Irrigation"], Name = "Water Pump", Description = "Irrigation water pump", Price = 2500000, Unit = "unit", Stock = 25, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Soil Amendments"], Name = "Lime Powder", Description = "Improve soil pH", Price = 90000, Unit = "bag", Stock = 60, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Compost"], Name = "Organic Compost", Description = "Natural compost fertilizer", Price = 70000, Unit = "bag", Stock = 100, Status = "Active" },

            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Spices and Herbs"], Name = "Black Pepper", Description = "Dried pepper seeds", Price = 120000, Unit = "kg", Stock = 40, Status = "Active" },
            new Product { ProductId = Guid.NewGuid(), CategoryId = categoryDict["Plantation Crops"], Name = "Coffee Beans", Description = "Raw coffee beans", Price = 130000, Unit = "kg", Stock = 80, Status = "Active" }
            };

            // Get existing product names
            var existingNamesProduct = await context.Set<Product>()
                .Select(p => p.Name)
                .ToListAsync();

            // Insert only new ones
            var newProducts = products
                .Where(p => !existingNamesProduct.Contains(p.Name))
                .ToList();

            if (newProducts.Any())
            {
                context.Set<Product>().AddRange(newProducts);
                await context.SaveChangesAsync();
            }

            //PaymentMethod seeding
            if (true)
            {
                var paymentMethods = new List<PaymentMethod>
                {
                    new PaymentMethod 
                    {
                        MethodName = "Cash On Delivery" 
                    },
                    new PaymentMethod 
                    {
                        MethodName = "VNPay" 
                    },
                };

                var existPaymentMethods = await context.Set<PaymentMethod>()
                                            .Where(pm => paymentMethods.Select(p => p.MethodName).Contains(pm.MethodName))
                                            .ToListAsync();

                var newPaymentMethods = paymentMethods
                                    .Where(pm => !existPaymentMethods.Any(epm => epm.MethodName == pm.MethodName))
                                    .ToList();

                context.Set<PaymentMethod>().AddRange(newPaymentMethods);
                await context.SaveChangesAsync();
            }

            //DeliveryFee Seeding
            if (true)
            {
                var deliveryFees = new List<DeliveryFee>
                {
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 1", District = "Any", FeeAmount = 5000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 2", District = "Any", FeeAmount = 10000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 3", District = "Any", FeeAmount = 15000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 4", District = "Any", FeeAmount = 20000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 5", District = "Any", FeeAmount = 25000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 6", District = "Any", FeeAmount = 30000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 7", District = "Any", FeeAmount = 35000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 8", District = "Any", FeeAmount = 40000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 9", District = "Any", FeeAmount = 45000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 10", District = "Any", FeeAmount = 50000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 11", District = "Any", FeeAmount = 55000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận 12", District = "Any", FeeAmount = 60000 },

                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Bình Thạnh", District = "Any", FeeAmount = 65000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Gò Vấp", District = "Any", FeeAmount = 70000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Phú Nhuận", District = "Any", FeeAmount = 75000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Tân Bình", District = "Any", FeeAmount = 80000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Tân Phú", District = "Any", FeeAmount = 85000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Quận Bình Tân", District = "Any", FeeAmount = 90000 },

                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Huyện Bình Chánh", District = "Any", FeeAmount = 95000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Huyện Củ Chi", District = "Any", FeeAmount = 100000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Huyện Hóc Môn", District = "Any", FeeAmount = 105000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Huyện Nhà Bè", District = "Any", FeeAmount = 110000 },
                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Huyện Cần Giờ", District = "Any", FeeAmount = 115000 },

                    new DeliveryFee { DeliveryFeeId = Guid.NewGuid(), City = "HCM", Ward = "Thành phố Thủ Đức", District = "Any", FeeAmount = 120000 }
                };

                // get wards already in database
                var existingWards = await context.DeliveryFees
                    .Select(x => x.Ward)
                    .ToHashSetAsync();

                // keep only rows that don't exist
                var feesToInsert = deliveryFees
                    .Where(x => !existingWards.Contains(x.Ward))
                    .ToList();

                if (feesToInsert.Any())
                {
                    context.DeliveryFees.AddRange(feesToInsert);
                    await context.SaveChangesAsync();
                }
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

            //Order Seeding
            if (!await context.Set<Order>().AnyAsync())
            {
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    PaymentMethodId = 1,
                    CreatedAt = DateTime.UtcNow,
                    PaymentStatus = "Pending",
                    TotalAmount = 10400,
                };

                context.Payments.Add(payment);
                await context.SaveChangesAsync();

                var order = new Order
                {
                    OrderId = Guid.NewGuid(),
                    CustomerId = await context.Set<Account>()
                                            .Where(a => a.Email == "user@gmail.com")
                                            .Select(a => a.UserProfile.UserProfileId)
                                            .FirstOrDefaultAsync(),

                    AddressId = await context.Set<Account>()
                                            .Where(a => a.Email == "user@gmail.com")
                                            .Select(a => a.UserProfile.Addresses.Where(a => a.AddressDetail == "123")
                                                                                .Select(a => a.AddressId)
                                                                                .FirstOrDefault())
                                            .FirstOrDefaultAsync(),

                    DeliveryFeeId = await context.Set<DeliveryFee>()
                                                .Where(df => df.Ward == "Quận 8")
                                                .Select(df => df.DeliveryFeeId)
                                                .FirstOrDefaultAsync(),

                    PaymentId = payment.PaymentId,
                    OrderDate = DateTime.UtcNow,
                    OrderStatus = "Pending"
                };

                context.Orders.Add(order);
                await context.SaveChangesAsync();

                var orderDetail = new OrderDetail
                {
                    OrderDetailId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    ProductId = await context.Set<Product>()
                                            .Where(p => p.Name == "Peach")
                                            .Select(p => p.ProductId)
                                            .FirstOrDefaultAsync(),
                    Quantity = 1,
                    TotalPrice = 10400,
                    CreatedAt = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(30),
                };

                context.OrderDetails.Add(orderDetail);
                await context.SaveChangesAsync();
            }
        }
    }
}
