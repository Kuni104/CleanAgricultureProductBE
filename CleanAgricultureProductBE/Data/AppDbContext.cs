using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
        public DbSet<Role> Role => Set<Role>();

        public DbSet<Schedule> Schedules => Set<Schedule>();

        public DbSet<Complaint> Complaints => Set<Complaint>();

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
                      .HasForeignKey<UserProfile>(p => p.UserProfileId);


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
        }
    }
}
