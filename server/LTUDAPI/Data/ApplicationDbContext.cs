using Microsoft.EntityFrameworkCore;
using LTUDAPI.Models;

namespace LTUDAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<GiangVien> GiangViens { get; set; }
        public DbSet<DanhGia> DanhGias { get; set; }
        public DbSet<ReminderConfig> ReminderConfigs { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>().ToTable("ACCOUNT").HasKey(a => a.IdAcc);
            modelBuilder.Entity<User>().ToTable("USER").HasKey(u => u.IdAcc);
            modelBuilder.Entity<GiangVien>().ToTable("GIANG_VIEN").HasKey(g => g.IdGiangVien);
            modelBuilder.Entity<DanhGia>().ToTable("DANH_GIA").HasKey(d => d.IdDanhGia);
            modelBuilder.Entity<ReminderConfig>().ToTable("REMINDER_CONFIG").HasKey(r => r.IdConfig);
            modelBuilder.Entity<UserLog>().ToTable("USER_LOG").HasKey(l => l.IdLog);

            // Cấu hình quan hệ 1-1 giữa Account và User
            modelBuilder.Entity<Account>()
                .HasOne<User>()
                .WithOne()
                .HasForeignKey<User>(u => u.IdAcc);
        }
    }
}