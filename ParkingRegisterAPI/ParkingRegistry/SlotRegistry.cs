using ParkingRegisterAPI.Models;

namespace ParkingRegisterAPI.ParkingRegistry
{
    public class SlotRegistry
    {
        public static void SetupLots(int numberOfLots)
        {
            for (int i = 1; i <= numberOfLots; i++)
            {
                Slot lot = new Slot
                {
                    Number = i.ToString()
                };
                __slots[lot.Number] = lot;
            }
        }

        private static Dictionary<string, Slot> __slots { get; set; } = new Dictionary<string, Slot>();

        public static string FindEmptySlotNumber()
        {
            return __slots.FirstOrDefault(lotKvp => lotKvp.Value.IsEmpty()).Key;
        }

        public static void ParkCar(string slotNumber, Car car)
        {
            __slots[slotNumber].ParkCar(car);
        }

        public static void UnparkCar(string slotNumber)
        {
            __slots[slotNumber].UnparkCar();
        }

        public static Slot GetSlot(string slotNumber)
        {
            if (!__slots.ContainsKey(slotNumber)) return null;
            return __slots[slotNumber];
        }
    }
}
