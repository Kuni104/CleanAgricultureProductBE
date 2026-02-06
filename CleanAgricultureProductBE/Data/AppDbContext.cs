using CleanAgricultureProductBE.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Schedule> Schedules => Set<Schedule>();
        public DbSet<CycleSchedule> CycleSchedules => Set<CycleSchedule>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<ProductComplaint> ProductComplaints => Set<ProductComplaint>();
        public DbSet<Address> Addresses => Set<Address>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ProductImage> ProductImages => Set<ProductImage>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
        public DbSet<DeliveryFee> DeliveryFees => Set<DeliveryFee>();
        public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
        public DbSet<Payment> Payments => Set<Payment>();
        // Blacklisted tokens for logout/revocation
        public DbSet<BlackListedToken> BlacklistedTokens => Set<BlackListedToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasOne(a => a.Role)
                      .WithMany(r => r.Accounts)
                      .HasForeignKey(a => a.RoleId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.UserProfile)
                      .WithOne(up => up.Account)
                      .HasForeignKey<UserProfile>(p => p.AccountId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<UserProfile>(entity =>
            {
                entity.HasOne(a => a.Cart)
                      .WithOne(up => up.Customer)
                      .HasForeignKey<Cart>(p => p.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Schedule>(entity =>
            {
                entity.HasOne(a => a.DeliveryPerson)
                      .WithMany(r => r.Schedules)
                      .HasForeignKey(a => a.DeliveryPersonId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Complaint>(entity =>
            {
                entity.HasOne(a => a.Staff)
                      .WithMany(r => r.Complaints)
                      .HasForeignKey(a => a.StaffId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Address>(entity =>
            {
                entity.HasOne(a => a.UserProfile)
                      .WithMany(r => r.Addresses)
                      .HasForeignKey(a => a.UserProfileId)
                      .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasOne(a => a.Category)
                      .WithMany(r => r.Products)
                      .HasForeignKey(a => a.CategoryId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.Price)
                      .HasPrecision(18, 2);

            });

            modelBuilder.Entity<ProductImage>(entity =>
            {
                entity.HasOne(a => a.Product)
                      .WithMany(r => r.ProductImages)
                      .HasForeignKey(a => a.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasOne(a => a.PaymentMethod)
                      .WithMany(r => r.Payments)
                      .HasForeignKey(a => a.PaymentMethodId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.Property(p => p.TotalAmount)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<DeliveryFee>(entity =>
            {
                entity.Property(df => df.FeeAmount)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(a => a.Customer)
                      .WithMany(r => r.Orders)
                      .HasForeignKey(a => a.CustomerId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Address)
                      .WithMany(r => r.Orders)
                      .HasForeignKey(a => a.AddressId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Schedule)
                       .WithMany(r => r.Orders)
                       .HasForeignKey(a => a.ScheduleId)
                       .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.DeliveryFee)
                       .WithMany(r => r.Orders)
                       .HasForeignKey(a => a.DeliveryFeeId)
                       .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Payment)
                       .WithMany(r => r.Orders)
                       .HasForeignKey(a => a.PaymentId)
                       .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Complaint)
                       .WithOne(c => c.Order)
                       .HasForeignKey<Complaint>(c => c.OrderId)
                       .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasOne(a => a.Cart)
                      .WithMany(r => r.CartItems)
                      .HasForeignKey(a => a.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Product)
                      .WithMany(r => r.CartItems)
                      .HasForeignKey(a => a.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.CartId, e.ProductId })
                      .IsUnique();
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasOne(a => a.Order)
                      .WithMany(r => r.OrderDetails)
                      .HasForeignKey(a => a.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Product)
                      .WithMany(r => r.OrderDetails)
                      .HasForeignKey(a => a.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.OrderId, e.ProductId })
                      .IsUnique();

                entity.Property(od => od.TotalPrice)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<ProductComplaint>(entity =>
            {
                entity.HasOne(a => a.Complaint)
                      .WithMany(r => r.ProductComplaints)
                      .HasForeignKey(a => a.ComplaintId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Product)
                      .WithMany(r => r.ProductComplaints)
                      .HasForeignKey(a => a.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(e => new { e.ComplaintId, e.ProductId })
                      .IsUnique();
            });

            modelBuilder.Entity<CycleSchedule>(entity =>
            {
                entity.HasOne(a => a.Order)
                      .WithMany(r => r.CycleSchedules)
                      .HasForeignKey(a => a.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            /*------------------------------------------------------------------------------------------------------------------------*/

            // Seed initial data
            modelBuilder.Entity<Role>().HasData(
                new Role { RoleId = 1, RoleName = "Admin" },
                new Role { RoleId = 2, RoleName = "Customer" },
                new Role { RoleId = 3, RoleName = "Staff" },
                new Role { RoleId = 4, RoleName = "DeliveryPerson" }
            );
        }
    }
}
