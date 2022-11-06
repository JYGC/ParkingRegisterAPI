namespace ParkingRegisterAPIFS.ParkingRegistry

open ParkingRegisterAPIFS.Models.CarInSlot
open ParkingRegisterAPIFS.Models.Slot
open ParkingRegisterAPIFS.Models.Car
open System

module SlotRegistry =
    type ISlotRegistry =
        abstract member findEmptySlotNumber: unit -> string
        abstract member parkCar: slotNumber: string * Car -> unit
        abstract member unparkCar: string -> unit
        abstract member getSlot: string -> Nullable<Slot>


    type LotSizeInfo() =
        static let mutable __numberOfLots: int = 0
        
        static member setupLots(numberOfLots: int) =
            __numberOfLots <- numberOfLots

        static member getNumberOfLots() =
            __numberOfLots


    type SlotRegistry() =
        inherit LotSizeInfo()

        let mutable __slots = Lazy<Map<string,Slot>>.Create(
            fun() -> seq {for slotNumber in 1 .. LotSizeInfo.getNumberOfLots() do yield new Slot(string slotNumber)} |>
                        Seq.map(fun slot -> slot.Number, slot) |>
                        Map.ofSeq)

        let changeSlot(slotNumber: string, carInSlot: Nullable<CarInSlot>) =
            Lazy<Map<string,Slot>>(__slots.Value |> Map.change slotNumber (
                fun slot -> match slot with
                            | Some value -> Some (Slot(value.Number, carInSlot))
                            | None -> None))


        interface ISlotRegistry with
            member _.findEmptySlotNumber() =
                __slots.Value |> Map.tryFindKey (fun _ value -> not value.CarInSlot.HasValue) |> Option.defaultValue ""

            member _.parkCar(slotNumber: string, car: Car) =
                __slots <- changeSlot(slotNumber, car.CarInSlot)

            member _.unparkCar(slotNumber: string) =
                __slots <- changeSlot(slotNumber, Nullable<CarInSlot>())

            member _.getSlot(slotNumber: string) =
                if __slots.Value |> Map.containsKey slotNumber then
                    Nullable<Slot>(__slots.Value.[slotNumber])
                else
                    Nullable<Slot>()
