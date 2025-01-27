namespace BookingSalonApp.Models

{
    public class Salon
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Location { get; set; }
        public string WorkingHours { get; set; }
        public string ImagePath { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Service> Services { get; set; }
    }
}
