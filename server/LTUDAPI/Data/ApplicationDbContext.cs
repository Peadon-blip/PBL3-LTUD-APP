using Microsoft.EntityFrameworkCore;
using ReminderAPI.Models;

namespace ReminderAPI.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ReminderConfig> ReminderConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Account>().ToTable("ACCOUNT").HasKey(a => a.IdAcc);
            modelBuilder.Entity<ReminderConfig>().ToTable("REMINDER_CONFIG").HasKey(r => r.IdConfig);
            modelBuilder.Entity<Account>()
                .HasOne(a => a.User)
                .WithOne()
                .HasForeignKey<User>(u => u.IdAcc);
        }
    }
}