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
    public List<WorkingHour> WorkingHours { get; set; }
    public List<int> SelectedServices { get; set; }
    public int? UserId { get; set; }

}
