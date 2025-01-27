namespace BookingSalonApp.Models
{
    public class BookingViewModel
    {
        public int SalonId { get; set; }
        public string SalonName { get; set; }
        public int EmployeeId { get; set; }
        public int UserId { get; set; }

        public DateTime Date { get; set; } 
        public IEnumerable<Employee> Employees { get; set; } 
    }
}
