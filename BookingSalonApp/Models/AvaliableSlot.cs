namespace BookingSalonApp.Models
{
    public class AvailableSlot
    {
        public int Id { get; set; }

        // Start and End times for the available slot
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Foreign keys to Employee and optionally Salon
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

        // Optionally, you could link the available slot to a specific Salon if needed
        public int? SalonId { get; set; }
        public Salon Salon { get; set; }

        // Mark if the slot is still available
        public bool IsAvailable { get; set; } = true;
    }
}
