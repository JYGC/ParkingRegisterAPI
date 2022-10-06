using Microsoft.AspNetCore.Mvc;
using ParkingRegisterAPI.Models;
using ParkingRegisterAPI.ParkingRegistry;
using ParkingRegisterAPI.Responses;

namespace ParkingRegisterAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ParkingRegisterController : ControllerBase
    {
        private const int __MAXREQUESTS = 10;
        private const int __TIMEWINDOW = 10;

        public ParkingRegisterController() { }

        [HttpPost("ParkCar")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public ParkCarResponse ParkCar(string carNumber)
        {
            if (CarRegistry.GetCar(carNumber) != null) return new ParkCarResponse { ErrorMesssage = "car is already parked in a lot" };
            // find lot and park if exists
            string slotNumber = SlotRegistry.FindEmptySlotNumber();
            if (slotNumber == null) return new ParkCarResponse { ErrorMesssage = "car park is full" };
            // register car and it's lot
            Car newCar = new Car
            {
                Number = carNumber,
                Slot = SlotRegistry.GetSlot(slotNumber)
            };
            SlotRegistry.ParkCar(slotNumber, newCar);
            CarRegistry.RegisterCar(newCar);
            return new ParkCarResponse { SlotNumber = slotNumber };
        }

        [HttpDelete("UnparkCar")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public UnparkCarResponse UnparkCar(string slotNumber)
        {
            Slot slot = SlotRegistry.GetSlot(slotNumber);
            if (slot == null) return new UnparkCarResponse { ErrorMesssage = "no slot exists" };
            if (slot.IsEmpty()) return new UnparkCarResponse { ErrorMesssage = $"slot {slotNumber} has no car" };
            string carNumber = slot.Car.Number;
            SlotRegistry.UnparkCar(slotNumber);
            CarRegistry.DeregisterCar(carNumber);
            return new UnparkCarResponse { Success = true };
        }

        [HttpGet("CarSlotInfo")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public CarSlotInfoResponse CarSlotInfo(string? slotNumber, string? carNumber)
        {
            if (string.IsNullOrEmpty(slotNumber))
            {
                Car car = CarRegistry.GetCar(carNumber);
                if (car == null) return new CarSlotInfoResponse { ErrorMessage = "car not found" };
                return new CarSlotInfoResponse { SlotNumber = car.Slot.Number, CarNumber = carNumber };
            }

            Slot slot = SlotRegistry.GetSlot(slotNumber);
            if (slot == null) return new CarSlotInfoResponse { ErrorMessage = "slot not found" };
            string carNumberFromSlot = slot.IsEmpty() ? "" : slot.Car.Number;
            if (!string.IsNullOrEmpty(carNumber) && carNumberFromSlot != carNumber) 
                return new CarSlotInfoResponse { ErrorMessage = $"car with number {carNumber} does not exists in slot" };
            return new CarSlotInfoResponse { SlotNumber = slotNumber, CarNumber = carNumberFromSlot };
        }
    }
}