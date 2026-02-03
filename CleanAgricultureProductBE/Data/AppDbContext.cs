using CleanAgricultureProductBE.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanAgricultureProductBE.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Role> Role => Set<Role>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
                entity.HasOne(a => a.Role)
                      .WithMany(r => r.Accounts)
                      .HasForeignKey(a => a.RoleId)
                      .OnDelete(DeleteBehavior.Cascade)
            );
        }
    }
}
