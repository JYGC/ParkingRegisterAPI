namespace ParkingRegisterAPIFS.Models

open System

module CarInSlot =
    type CarInSlot = struct
        val mutable CarNumber: string
        val mutable SlotNumber: string

        new (carNumber: string, slotNumber: string) =
            { CarNumber = carNumber; SlotNumber = slotNumber }
    end

module Car =
    type Car = struct
        val Number: string
        val mutable CarInSlot: Nullable<CarInSlot.CarInSlot>

        new (number: string)=
            { Number = number; CarInSlot = Nullable<CarInSlot.CarInSlot>(); }
        new (number: string, carInSlot: Nullable<CarInSlot.CarInSlot>) =
            { Number = number; CarInSlot = carInSlot; }
    end

module Slot =
    type Slot = struct
        val Number: string
        val mutable CarInSlot: Nullable<CarInSlot.CarInSlot>

        new (number: string) =
            { Number = number; CarInSlot = Nullable<CarInSlot.CarInSlot>(); }
        new (number: string, carInSlot: Nullable<CarInSlot.CarInSlot>) =
            { Number = number; CarInSlot = carInSlot; }
    end
