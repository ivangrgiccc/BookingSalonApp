using System;
using System.ComponentModel.DataAnnotations;
using BookingSalonApp.Models;


namespace BookingSalonApp.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }
        [Required]

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }


        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
