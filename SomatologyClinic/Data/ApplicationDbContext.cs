using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SomatologyClinic.Models;

namespace SomatologyClinic.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Treatment> Treatments { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Student>(entity =>
            {
                entity.ToTable("Students");
                entity.HasIndex(e => e.StudentNumber).IsUnique();
                entity.HasOne(s => s.ApplicationUser)
                    .WithOne()
                    .HasForeignKey<Student>(s => s.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Staff>(entity =>
            {
                entity.ToTable("Staffs");
                entity.HasIndex(e => e.StaffId).IsUnique();
                entity.HasOne(s => s.ApplicationUser)
                    .WithOne()
                    .HasForeignKey<Staff>(s => s.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasIndex(e => e.PhoneNumber);
                entity.HasOne(c => c.ApplicationUser)
                    .WithOne()
                    .HasForeignKey<Customer>(c => c.ApplicationUserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Appointment>(entity =>
            {
                entity.HasOne(a => a.User)
                    .WithMany(u => u.Appointments)
                    .HasForeignKey(a => a.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(a => a.Treatment)
                    .WithMany(t => t.Appointments)
                    .HasForeignKey(a => a.TreatmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Payment>(entity =>
            {
                entity.Property(p => p.Amount).HasColumnType("decimal(18,2)");
                entity.HasOne(p => p.ApplicationUser)
                    .WithMany(u => u.Payments)
                    .HasForeignKey(p => p.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<Treatment>(entity =>
            {
                entity.Property(t => t.Price).HasColumnType("decimal(18,2)");
                entity.HasMany(t => t.Appointments)
                    .WithOne(a => a.Treatment)
                    .HasForeignKey(a => a.TreatmentId)
                    .OnDelete(DeleteBehavior.Restrict);
                entity.HasMany(t => t.Bookings)
                    .WithOne(b => b.Treatment)
                    .HasForeignKey(b => b.TreatmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


        builder.Entity<Booking>(entity =>
            {
                entity.HasKey(b => b.Id);

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Bookings)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(b => b.Treatment)
                    .WithMany(t => t.Bookings)
                    .HasForeignKey(b => b.TreatmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.Property(b => b.BookingDateTime)
                    .IsRequired();

                entity.Property(b => b.Status)
                    .IsRequired()
                    .HasConversion<string>();

                entity.Property(b => b.StaffNotes)
                    .HasMaxLength(500);

                entity.Property(b => b.LastUpdatedBy)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.Property(b => b.LastUpdatedAt)
                    .IsRequired();

                entity.HasIndex(b => b.BookingDateTime);
            });
        }
    }
}
