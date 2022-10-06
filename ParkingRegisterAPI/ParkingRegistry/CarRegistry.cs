using ParkingRegisterAPI.Models;

namespace ParkingRegisterAPI.ParkingRegistry
{
    public interface ICarRegistry
    {
        Car GetCar(string carNumber);
        void RegisterCar(Car car);
        void DeregisterCar(string carNumber);
    }

    public class CarRegistry : ICarRegistry
    {
        private Dictionary<string, Car> __cars { get; set; } = new Dictionary<string, Car>();

        public Car GetCar(string carNumber)
        {
            if (!__cars.ContainsKey(carNumber)) return null;
            return __cars[carNumber];
        }

        public void RegisterCar(Car car)
        {
            __cars[car.Number] = car;
        }

        public void DeregisterCar(string carNumber)
        {
            __cars.Remove(carNumber);
        }
    }
}
