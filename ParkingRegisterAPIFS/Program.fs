namespace ParkingRegisterAPIFS

open ParkingRegisterAPIFS.ParkingRegistry.SlotRegistry
open ParkingRegisterAPIFS.ParkingRegistry.CarRegistry

#nowarn "20"
open System
open System.Collections.Generic
open System.IO
open System.Linq
open System.Threading.Tasks
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.HttpsPolicy
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

module Program =
    let exitCode = 0

    [<EntryPoint>]
    let main args =

        let builder = WebApplication.CreateBuilder(args)

        LotSizeInfo.setupLots(builder.Configuration.GetValue<int>("ParkingLotSize"))
        builder.Services.AddSingleton<ISlotRegistry, SlotRegistry>()
        builder.Services.AddSingleton<ICarRegistry, CarRegistry>()

        builder.Services.AddControllers()

        builder.Services.AddEndpointsApiExplorer()
        builder.Services.AddSwaggerGen()

        let app = builder.Build()

        app.UseHttpsRedirection()

        if app.Environment.IsDevelopment() then
            app.UseSwagger()
            app.UseSwaggerUI() |> ignore

        app.UseAuthorization()
        app.MapControllers()

        app.Run()

        exitCode
