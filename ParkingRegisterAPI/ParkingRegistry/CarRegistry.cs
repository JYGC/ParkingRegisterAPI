using ParkingRegisterAPI.Models;

namespace ParkingRegisterAPI.ParkingRegistry
{
    public class CarRegistry
    {
        private static Dictionary<string, Car> __cars { get; set; } = new Dictionary<string, Car>();

        public static Car GetCar(string carNumber)
        {
            if (!__cars.ContainsKey(carNumber)) return null;
            return __cars[carNumber];
        }

        public static void RegisterCar(Car car)
        {
            __cars[car.Number] = car;
        }

        public static void DeregisterCar(string carNumber)
        {
            __cars.Remove(carNumber);
        }
    }
}
