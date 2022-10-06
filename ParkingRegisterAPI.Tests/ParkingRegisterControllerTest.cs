using System.Text.Json;
using ParkingRegisterAPI.Controllers;
using ParkingRegisterAPI.Responses;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace ParkingRegisterAPI.Tests
{
    public class AlphabeticalOrderer : ITestCaseOrderer
    {
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases)
                where TTestCase : ITestCase
        {
            var result = testCases.ToList();
            result.Sort((x, y) => StringComparer.OrdinalIgnoreCase.Compare(x.TestMethod.Method.Name, y.TestMethod.Method.Name));
            return result;
        }
    }

    [TestCaseOrderer("TestOrderExamples.TestCaseOrdering.AlphabeticalOrderer", "TestOrderExamples")]
    public class ParkingRegisterControllerTest
    {
        private readonly ParkingRegisterController __controller;

        public ParkingRegisterControllerTest()
        {
            ParkingRegistry.SlotRegistry.SetupLots(5);
            __controller = new ParkingRegisterController();
        }

        [Fact]
        public void Park1stCar()
        {
            var result = __controller.ParkCar("ADW635");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "1"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void Park2ndCar()
        {
            __controller.ParkCar("ADW635");
            var result = __controller.ParkCar("XBd37w");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "2"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void Park3rdCar()
        {
            __controller.ParkCar("ADW635");
            __controller.ParkCar("XBd37w");
            var result = __controller.ParkCar("XBA37w");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "3"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void Park4thCar()
        {
            __controller.ParkCar("ADW635");
            __controller.ParkCar("XBd37w");
            __controller.ParkCar("XBA37w");
            var result = __controller.ParkCar("XBA376");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "4"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void Park5thCar()
        {
            __controller.ParkCar("ADW635");
            __controller.ParkCar("XBd37w");
            __controller.ParkCar("XBA37w");
            __controller.ParkCar("XBA376");
            var result = __controller.ParkCar("VIC467");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "5"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TrPark6thCar()
        {
            __controller.ParkCar("ADW635");
            __controller.ParkCar("XBd37w");
            __controller.ParkCar("XBA37w");
            __controller.ParkCar("XBA376");
            __controller.ParkCar("VIC467");
            var result = __controller.ParkCar("ertt32");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                ErrorMesssage = "car park is full"
            }), JsonSerializer.Serialize(result));
        }

        //[Fact]
        //public void ExceedRateLimitOf10()
        //{
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    __controller.CarSlotInfo("3333", "3333");
        //    var result = __controller.CarSlotInfo("3333", "3333");
        //    Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
        //    {
        //        ErrorMessage = "slot not found"
        //    }), JsonSerializer.Serialize(result));
        //}
    }
}