using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookingSalonApp.Models;

namespace BookingSalonApp.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Salon> Salons { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ReservationService> ReservationServices { get; set; }
        public DbSet<WorkingHour> WorkingHours { get; set; } 
        public DbSet<AvailableSlot> AvailableSlots { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); 

            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Salon)
                .WithMany(s => s.Employees) 
                .HasForeignKey(e => e.SalonId)
                .OnDelete(DeleteBehavior.Cascade); 

            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Definiraj odnos Reservation → Salon
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Salon)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SalonId)
                .OnDelete(DeleteBehavior.NoAction);

            // Definiraj odnos Reservation → Employee
            modelBuilder.Entity<Reservation>()
                .HasOne(r => r.Employee)
                .WithMany(e => e.Reservations)
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);

            // M-N odnos (Reservation ↔ Service) pomoću ReservationService
            modelBuilder.Entity<ReservationService>()
                .HasKey(rs => new { rs.ReservationId, rs.ServiceId });  // Kompozitni ključ

            modelBuilder.Entity<ReservationService>()
                .HasOne(rs => rs.Reservation)
                .WithMany(r => r.ReservationServices)
                .HasForeignKey(rs => rs.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationService>()
                .HasOne(rs => rs.Service)
                .WithMany(s => s.ReservationServices)
                .HasForeignKey(rs => rs.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Definiraj odnos WorkingHour → Employee
            modelBuilder.Entity<WorkingHour>()
                .HasOne(wh => wh.Employee)
                .WithMany(e => e.WorkingHours) // Poveži radno vrijeme s zaposlenikom
                .HasForeignKey(wh => wh.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade); // Ako se zaposlenik obriše, brišu se i radna vremena

            // Osiguraj preciznost cijene
            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // Optimiziraj upite pomoću indeksa
            modelBuilder.Entity<Reservation>().HasIndex(r => r.UserId);
            modelBuilder.Entity<Reservation>().HasIndex(r => r.SalonId);
            modelBuilder.Entity<Reservation>().HasIndex(r => r.EmployeeId);
        }
    }
}
