using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class Salon
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ime salona je obavezno.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Lokacija je obavezna")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Adresa je obavezna.")]
        public string Address { get; set; }

        public int? WorkingHoursId { get; set; } 

        public List<WorkingHour> WorkingHours { get; set; } = new List<WorkingHour>();

        public string GoogleMapsIframe { get; set; }
        public string? ImagePath { get; set; }
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

        public List<Employee>? Employees { get; set; } = new List<Employee>();
        public List<Service> Services { get; set; } = new List<Service>();
        public TimeSpan OpeningTime { get; set; } = new TimeSpan(8, 0, 0); 
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(20, 0, 0); 
    }
}
