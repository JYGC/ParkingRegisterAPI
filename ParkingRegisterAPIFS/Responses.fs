namespace ParkingRegisterAPIFS.Responses

module ParkCarResponse =
    type ParkCarResponse = struct
        val SlotNumber: string
        val ErrorMessage: string

        new (slotNumber: string, errorMessage: string) = { SlotNumber = slotNumber; ErrorMessage = errorMessage; }
    end

module UnparkCarResponse =
    type UnparkCarResponse = struct
        val Success: bool
        val ErrorMessage: string

        new (success: bool, errorMessage: string) = { Success = success; ErrorMessage = errorMessage; }
    end

module CarSlotInfoResponse =
    type CarSlotInfoResponse = struct
        val SlotNumber: string
        val CarNumber: string
        val ErrorMessage: string

        new (slotNumber: string, carNumber: string, errorMessage: string) = { SlotNumber = slotNumber; CarNumber = carNumber; ErrorMessage = errorMessage; }
    end
