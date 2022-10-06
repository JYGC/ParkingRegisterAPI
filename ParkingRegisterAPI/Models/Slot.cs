using ParkingRegisterAPI.ParkingRegistry;

namespace ParkingRegisterAPI.Models
{
    public class Slot
    {
        public string Number { get; set; } = "";
        public Car? Car { get; set; }
        
        public bool IsEmpty()
        {
            return Car == null;
        }

        public void ParkCar(Car car)
        {
            Car = car;
        }

        public void UnparkCar()
        {
            Car = null;
        }
    }
}
