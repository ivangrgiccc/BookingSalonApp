namespace BookingSalonApp.Models
{
    public class AvailableSlot
    {
        public int Id { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int? SalonId { get; set; }
        public Salon Salon { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
