using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Employee name is required.")]
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public int SalonId { get; set; }
        public Salon Salon { get; set; } 
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
