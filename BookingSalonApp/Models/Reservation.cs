using System;
using System.Collections.Generic;

namespace BookingSalonApp.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int SalonId { get; set; }
        public Salon Salon { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public DateTime Date { get; set; }

        public ICollection<ReservationService> ReservationServices { get; set; }
        public ICollection<Service> Services { get; set; }
    }
}
