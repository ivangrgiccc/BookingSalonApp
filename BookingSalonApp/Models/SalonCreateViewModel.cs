namespace BookingSalonApp.Models
{
    public class SalonCreateViewModel
    {
        public string Name { get; set; }
        public List<WorkingHourViewModel> WorkingHours { get; set; } = new List<WorkingHourViewModel>();
    }

    public class WorkingHourViewModel
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
