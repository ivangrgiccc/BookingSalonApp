using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace BookingSalonApp.Models
{
    public class Salon
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Salon name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Working hours are required.")]
        public string WorkingHours { get; set; }
        public string ImagePath { get; set; }
        public List<Employee> Employees { get; set; } = new List<Employee>();
        public List<Service> Services { get; set; } = new List<Service>();
    }
}
