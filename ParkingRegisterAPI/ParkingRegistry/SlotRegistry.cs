using ParkingRegisterAPI.Models;

namespace ParkingRegisterAPI.ParkingRegistry
{
    public interface ISlotRegistry
    {
        string FindEmptySlotNumber();
        void ParkCar(string slotNumber, Car car);
        void UnparkCar(string slotNumber);
        Slot GetSlot(string slotNumber);
    }

    public class LotSizeInfo
    {
        protected static int _numberOfLots;
        public static void SetupLots(int numberOfLots)
        {
            _numberOfLots = numberOfLots;
        }

    }


    public class SlotRegistry : LotSizeInfo, ISlotRegistry
    {
        private Dictionary<string, Slot> __slots { get; set; } = new Dictionary<string, Slot>();

        public SlotRegistry()
        {
            for (int i = 1; i <= _numberOfLots; i++)
            {
                Slot lot = new Slot
                {
                    Number = i.ToString()
                };
                __slots[lot.Number] = lot;
            }
        }

        public string FindEmptySlotNumber()
        {
            return __slots.FirstOrDefault(lotKvp => lotKvp.Value.IsEmpty()).Key;
        }

        public void ParkCar(string slotNumber, Car car)
        {
            __slots[slotNumber].ParkCar(car);
        }

        public void UnparkCar(string slotNumber)
        {
            __slots[slotNumber].UnparkCar();
        }

        public Slot GetSlot(string slotNumber)
        {
            if (!__slots.ContainsKey(slotNumber)) return null;
            return __slots[slotNumber];
        }
    }
}
