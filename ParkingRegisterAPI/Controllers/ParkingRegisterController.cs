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

        private readonly string __slotNotFoundMsg = "no slot exists";
        private readonly string __carNotFoundMsg = "car not found";
        private readonly string __carparkFullMsg = "car park is full";
        private readonly string __invalidCarNumMsg = "no valid car number given";
        private readonly string __carAlreadyInSlotMsg = "car is already parked in a slot";
        private readonly string __slotIsEmptyPlaceholderMsg = "slot {0} has no car";
        private readonly string __slotHasNoCarPlaceholderMsg = "slot {0} does not have car with number {1}";
        private readonly string __slotNumInvalidMsg = "slot number is invalid";
        private readonly string __carSlotNumInvalidMsg = "car and slot numbers are invalid";

        private ISlotRegistry __slotRegistry;
        private ICarRegistry __carRegistry;

        public ParkingRegisterController(ISlotRegistry slotRegistry, ICarRegistry carRegistry)
        {
            __slotRegistry = slotRegistry;
            __carRegistry = carRegistry;
        }

        [HttpPost("ParkCar")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public ParkCarResponse ParkCar(string carNumber)
        {
            if (string.IsNullOrWhiteSpace(carNumber)) return new ParkCarResponse { ErrorMessage = __invalidCarNumMsg };
            if (__carRegistry.GetCar(carNumber) != null) return new ParkCarResponse { ErrorMessage = __carAlreadyInSlotMsg };
            // find lot and park if exists
            string slotNumber = __slotRegistry.FindEmptySlotNumber();
            if (slotNumber == null) return new ParkCarResponse { ErrorMessage = __carparkFullMsg };
            // register car and it's lot
            Car newCar = new Car
            {
                Number = carNumber,
                Slot = __slotRegistry.GetSlot(slotNumber)
            };
            __slotRegistry.ParkCar(slotNumber, newCar);
            __carRegistry.RegisterCar(newCar);
            return new ParkCarResponse { SlotNumber = slotNumber };
        }

        [HttpDelete("UnparkCar")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public UnparkCarResponse UnparkCar(string slotNumber)
        {
            if (string.IsNullOrWhiteSpace(slotNumber)) return new UnparkCarResponse { ErrorMessage = __slotNumInvalidMsg };
            Slot slot = __slotRegistry.GetSlot(slotNumber);
            if (slot == null) return new UnparkCarResponse { ErrorMessage = __slotNotFoundMsg };
            if (slot.IsEmpty()) return new UnparkCarResponse { ErrorMessage = String.Format(__slotIsEmptyPlaceholderMsg, slotNumber) };
            string carNumber = slot.Car.Number;
            __slotRegistry.UnparkCar(slotNumber);
            __carRegistry.DeregisterCar(carNumber);
            return new UnparkCarResponse { Success = true };
        }

        [HttpGet("CarSlotInfo")]
        [LimitRequests(MaxRequests = __MAXREQUESTS, TimeWindow = __TIMEWINDOW)]
        public CarSlotInfoResponse CarSlotInfo(string? slotNumber, string? carNumber)
        {
            if (string.IsNullOrWhiteSpace(slotNumber))
            {
                if (string.IsNullOrWhiteSpace(carNumber)) return new CarSlotInfoResponse { ErrorMessage = __carSlotNumInvalidMsg };
                Car car = __carRegistry.GetCar(carNumber);
                if (car == null) return new CarSlotInfoResponse { ErrorMessage = __carNotFoundMsg };
                return new CarSlotInfoResponse { SlotNumber = car.Slot.Number, CarNumber = carNumber };
            }

            Slot slot = __slotRegistry.GetSlot(slotNumber);
            if (slot == null) return new CarSlotInfoResponse { ErrorMessage = __slotNotFoundMsg };
            string carNumberFromSlot = slot.IsEmpty() ? null : slot.Car.Number;
            if (!string.IsNullOrEmpty(carNumber) && carNumberFromSlot != carNumber) 
                return new CarSlotInfoResponse { ErrorMessage = String.Format(__slotHasNoCarPlaceholderMsg, slot.Number, carNumber) };
            return new CarSlotInfoResponse { SlotNumber = slotNumber, CarNumber = carNumberFromSlot };
        }
    }
}