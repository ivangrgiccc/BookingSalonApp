using System;
using System.Collections.Generic;

namespace BookingSalonApp.Models
{
    public class BookingViewModel
    {
        public int SalonId { get; set; }
        public string SalonName { get; set; }

        // Osiguravamo da lista zaposlenika nije null
        public List<Employee> Employees { get; set; } = new List<Employee>();

        public int? EmployeeId { get; set; }
        public string Date { get; set; }

        // Osiguravamo da lista odabranih usluga nije null
        public List<int> SelectedServices { get; set; } = new List<int>();

        // UserId može biti null jer korisnik možda nije prijavljen
        public int? UserId { get; set; }

        // Osiguravamo da lista usluga nije null
        public List<Service> Services { get; set; } = new List<Service>();
        public List<WorkingHour> WorkingHours { get; set; } // Dodajte WorkingHours ovdje

    }
}
