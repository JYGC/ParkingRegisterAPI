namespace ParkingRegisterAPIFS.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Logging
open ParkingRegisterAPIFS
open ParkingRegisterAPIFS.Models.Car
open ParkingRegisterAPIFS.Models.Slot
open ParkingRegisterAPIFS.Models.CarInSlot
open ParkingRegisterAPIFS.ParkingRegistry.SlotRegistry
open ParkingRegisterAPIFS.ParkingRegistry.CarRegistry
open ParkingRegisterAPIFS.Responses.ParkCarResponse
open ParkingRegisterAPIFS.Responses.UnparkCarResponse
open ParkingRegisterAPIFS.Responses.CarSlotInfoResponse

[<ApiController>]
[<Route("[controller]")>]
type ParkingRegisterController (slotRegistry: ISlotRegistry, carRegistry: ICarRegistry) =
    inherit ControllerBase()

    let __slotNotFoundMsg: string = "no slot exists";
    let __carNotFoundMsg: string = "car not found";
    let __carparkFullMsg: string = "car park is full";
    let __invalidCarNumMsg: string = "no valid car number given";
    let __carAlreadyInSlotMsg: string = "car is already parked in a slot";
    let __slotIsEmptyPlaceholderMsg: string = "slot {0} has no car";
    let __slotHasNoCarPlaceholderMsg: string = "slot {0} does not have car with number {1}";
    let __slotNumInvalidMsg: string = "slot number is invalid";
    let __carSlotNumInvalidMsg: string = "car and slot numbers are invalid";
    
    let __slotRegistry = slotRegistry;
    let __carRegistry = carRegistry;
    
    let getInfoByCarNumber(carNumber: string): CarSlotInfoResponse =
        let car: Nullable<Car> = __carRegistry.getCar(carNumber)
        if String.IsNullOrWhiteSpace carNumber then
            CarSlotInfoResponse("", "", __carSlotNumInvalidMsg)
        elif not car.HasValue then
            CarSlotInfoResponse("", "", __carNotFoundMsg)
        else
            CarSlotInfoResponse(car.Value.CarInSlot.Value.SlotNumber, carNumber, "")
    
    let getInfoBySlotNumberCheckCarNumber(slotNumber: string, carNumber: string): CarSlotInfoResponse =
        let slot: Nullable<Slot> = __slotRegistry.getSlot(slotNumber)
        if not slot.HasValue then
            let carNumberFromSlot: string =
                if slot.HasValue && slot.Value.CarInSlot.HasValue then slot.Value.CarInSlot.Value.CarNumber else ""
            if not (String.IsNullOrEmpty(carNumber)) && carNumberFromSlot <> carNumber then
                CarSlotInfoResponse("", "", String.Format(__slotHasNoCarPlaceholderMsg, slot.Value.Number, carNumber))
            else
                CarSlotInfoResponse(slotNumber, carNumberFromSlot, "")
        else
            CarSlotInfoResponse("", "", __slotNotFoundMsg)

    [<HttpPost("ParkCar")>]
    member _.parkCar(carNumber: string) =
        let slotNumber: string = __slotRegistry.findEmptySlotNumber();
        if String.IsNullOrWhiteSpace(carNumber) then
            ParkCarResponse("", __invalidCarNumMsg)
        elif __carRegistry.getCar(carNumber).HasValue then
            ParkCarResponse("", __carAlreadyInSlotMsg)
        elif String.IsNullOrEmpty(slotNumber) then
            ParkCarResponse("", __carparkFullMsg)
        else
            let newCar = Car(carNumber, CarInSlot(carNumber, slotNumber))
            __slotRegistry.parkCar(slotNumber, newCar)
            __carRegistry.registerCar(newCar)
            ParkCarResponse(slotNumber, "")

    [<HttpDelete("UnparkCar")>]
    member _.unparkCar(slotNumber: string) =
        let slot: Nullable<Slot> = __slotRegistry.getSlot(slotNumber)
        if String.IsNullOrWhiteSpace slotNumber then
            UnparkCarResponse(false, __slotNumInvalidMsg)
        elif not slot.HasValue then
            UnparkCarResponse(false, __slotNotFoundMsg)
        elif not slot.Value.CarInSlot.HasValue then
            UnparkCarResponse(false, String.Format(__slotIsEmptyPlaceholderMsg, slotNumber))
        else
            let carNumber = slot.Value.CarInSlot.Value.CarNumber
            __slotRegistry.unparkCar(slotNumber)
            __carRegistry.deregisterCar(carNumber)
            UnparkCarResponse(true, "")

    [<HttpGet("CarSlotInfo")>]
    member _.carSlotInfo(slotNumber: string, carNumber: string) =
        let slot: Nullable<Slot> = __slotRegistry.getSlot(slotNumber)
        if String.IsNullOrWhiteSpace slotNumber then
            getInfoByCarNumber(carNumber)
        else
            getInfoBySlotNumberCheckCarNumber(slotNumber, carNumber)
