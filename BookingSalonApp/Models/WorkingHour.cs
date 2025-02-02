using System;
using System.ComponentModel.DataAnnotations;

namespace BookingSalonApp.Models
{
    public class WorkingHour
    {
        public int Id { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }


        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
