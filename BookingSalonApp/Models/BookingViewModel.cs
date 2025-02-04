using BookingSalonApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class BookingViewModel
    {
        [Required]
        public int SalonId { get; set; } // Salon is required

        public string SalonName { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Service> Services { get; set; }

        [Required]
        public int? EmployeeId { get; set; } // Nullable Employee ID, make sure it's correctly handled

        public List<WorkingHour> WorkingHours { get; set; }
        public List<int> SelectedServices { get; set; }

        public int? UserId { get; set; }

        [Required]
        public DateTime Date { get; set; } // Date is required

        public string TimeSlot { get; set; } // Optional, can be null
    }
}