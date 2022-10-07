using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace ParkingRegisterAPI.Tests
{
    public class RateLimitingMiddlewareTest
    {
        [Fact]
        public async Task ExceedRateLimitOf10()
        {
            using var host = await new HostBuilder().ConfigureWebHost(webBuilder =>
            {
                webBuilder = WebHost.CreateDefaultBuilder();
                webBuilder
                    .UseTestServer()
                    .ConfigureServices(services =>
                    {
                        services.AddDistributedMemoryCache();
                    })
                    .Configure(app =>
                    {
                        app.UseRateLimiting();
                    });
                })
                .StartAsync();

            var response = await host.GetTestClient().GetAsync("/");
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //__controller.CarSlotInfo("3333", "3333");
            //var result = __controller.CarSlotInfo("3333", "3333");
            //Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            //{
            //    ErrorMessage = "slot not found"
            //}), JsonSerializer.Serialize(result));
        }
    }
}
