namespace ParkingRegisterAPI.Models
{
    public class Car
    {
        public string Number { get; set; } = "";
        public Slot? Slot { get; set; }
    }
}
