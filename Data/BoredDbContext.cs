using BoredWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace BoredWeb.Data;

public class BoredDbContext(DbContextOptions<BoredDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<ActivityBookingOrder> ActivityBookingOrders { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Complaint> Complaints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(u => u.Name).IsRequired().HasMaxLength(200);
            e.Property(u => u.Email).IsRequired().HasMaxLength(320);
            e.Property(u => u.PasswordHash).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(20);
            e.Property(u => u.Bio).HasMaxLength(1000);
            e.Property(u => u.Occupation).HasMaxLength(200);
            e.Property(u => u.LocationAddress).HasMaxLength(500);
            e.Property(u => u.Role).IsRequired().HasMaxLength(20).HasDefaultValue("user");
            e.Property(u => u.JoinedAt).HasDefaultValueSql("now()");

            e.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<Activity>(e =>
        {
            e.HasKey(a => a.Id);
            e.Property(a => a.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(a => a.Name).IsRequired().HasMaxLength(300);
            e.Property(a => a.Description).IsRequired();
            e.Property(a => a.Category).IsRequired().HasMaxLength(100);
            e.Property(a => a.Price).HasColumnType("decimal(10,2)");
            e.Property(a => a.Location).IsRequired().HasMaxLength(500);
            e.Property(a => a.ImageUrl).HasMaxLength(2000);
            e.Property(a => a.Status).IsRequired().HasMaxLength(20).HasDefaultValue("forming");
            e.Property(a => a.CancellationReason).HasMaxLength(1000);
            e.Property(a => a.CreatedAt).HasDefaultValueSql("now()");

            // Querying by status (forming/confirmed/etc.) and filtering by category/date are common
            e.HasIndex(a => a.Status);
            e.HasIndex(a => a.Category);
            e.HasIndex(a => a.ActivityDate);
        });

        modelBuilder.Entity<ActivityBookingOrder>(e =>
        {
            e.HasKey(o => o.Id);
            e.Property(o => o.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(o => o.PaymentStatus).IsRequired().HasMaxLength(20).HasDefaultValue("pending");
            e.Property(o => o.ConfirmationStatus).IsRequired().HasMaxLength(20);
            e.Property(o => o.ParticipantsName).HasColumnType("text[]");
            e.Property(o => o.ParticipantsEmail).HasColumnType("text[]");
            e.Property(o => o.CreatedAt).HasDefaultValueSql("now()");

            e.HasOne(o => o.User)
                .WithMany(u => u.BookingOrders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.Activity)
                .WithMany(a => a.BookingOrders)
                .HasForeignKey(o => o.ActivityId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-one: each booking maps to exactly one transaction
            e.HasOne(o => o.Transaction)
                .WithOne(t => t.BookingOrder)
                .HasForeignKey<ActivityBookingOrder>(o => o.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(o => o.UserId);
            e.HasIndex(o => o.ActivityId);
            e.HasIndex(o => o.TransactionId).IsUnique();
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(t => t.Type).IsRequired().HasMaxLength(20).HasDefaultValue("booking");
            e.Property(t => t.Amount).HasColumnType("decimal(10,2)");
            e.Property(t => t.Status).IsRequired().HasMaxLength(20);
            e.Property(t => t.Description).HasMaxLength(1000);
            e.Property(t => t.CreatedAt).HasDefaultValueSql("now()");

            e.HasOne(t => t.User)
                .WithMany(u => u.Transactions)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(t => t.UserId);
            e.HasIndex(t => t.ReferenceId);
            e.HasIndex(t => t.Status);
        });

        modelBuilder.Entity<Complaint>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.Subject).IsRequired().HasMaxLength(300);
            e.Property(c => c.Body).IsRequired();
            e.Property(c => c.Category).IsRequired().HasMaxLength(100);
            e.Property(c => c.Status).IsRequired().HasMaxLength(20).HasDefaultValue("open");
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");

            e.HasOne(c => c.User)
                .WithMany(u => u.Complaints)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasIndex(c => c.UserId);
            e.HasIndex(c => c.Status);
        });
    }
}
