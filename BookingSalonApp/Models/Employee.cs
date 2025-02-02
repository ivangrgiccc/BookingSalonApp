using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime djelatnika je obavezno")]
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public int SalonId { get; set; }

        [ValidateNever]
        public Salon Salon { get; set; }

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
        public List<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();

    }
}
