namespace BookingSalonApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImagePath { get; set; }
        public List<Reservation> Reservations { get; set; }
    }
}
