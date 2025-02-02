namespace BookingSalonApp.Models
{
    public class AvailableSlot
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}