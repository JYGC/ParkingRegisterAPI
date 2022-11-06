namespace ParkingRegisterAPIFS.ParkingRegistry

open ParkingRegisterAPIFS.Models.Car
open System

module CarRegistry =
    type ICarRegistry =
        abstract member getCar: string -> Nullable<Car>
        abstract member registerCar: Car -> unit
        abstract member deregisterCar: string -> unit

    type CarRegistry() =
        let mutable __cars = Lazy<Map<string,Car>>.Create(fun() -> Map.empty)

        interface ICarRegistry with
            member _.getCar(carNumber: string): Nullable<Car> =
                if __cars.Value |> Map.containsKey carNumber then
                    Nullable<Car>(__cars.Value.[carNumber])
                else
                    Nullable<Car>()

            member _.registerCar(car: Car): unit =
                __cars <- Lazy<Map<string,Car>>(__cars.Value |> Map.add car.Number car)

            member _.deregisterCar(carNumber: string): unit =
                __cars <- Lazy<Map<string,Car>>(__cars.Value |> Map.remove carNumber)
