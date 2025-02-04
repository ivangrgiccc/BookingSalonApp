using BookingSalonApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class BookingViewModel
{
    public int SalonId { get; set; }
    public string SalonName { get; set; }
    public List<Employee> Employees { get; set; }
    public List<Service> Services { get; set; }
    public int? EmployeeId { get; set; }
    public List<WorkingHours> WorkingHours { get; set; }
    public List<int> SelectedServices { get; set; }

    [Required(ErrorMessage = "Datum je obavezan.")]
    public string Date { get; set; } // OVO OSTAVLJAMO

    [Required(ErrorMessage = "Termin je obavezan.")]
    public string TimeSlot { get; set; }

    public int? UserId { get; set; }
}
