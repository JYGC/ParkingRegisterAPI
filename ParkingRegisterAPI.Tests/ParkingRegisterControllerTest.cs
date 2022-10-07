using System.Text.Json;
using ParkingRegisterAPI.Controllers;
using ParkingRegisterAPI.Responses;
using ParkingRegisterAPI.ParkingRegistry;
using System;

namespace ParkingRegisterAPI.Tests
{
    public class ParkingRegisterControllerTest
    {
        const int __NUMBEROFSLOTS = 9999;
        const int __NUMBEROFCARS = 99999;
        const int __CARNUMBERLENGTH = 6;

        private readonly string[] __carNumbers;
        private readonly ParkingRegisterController __controller;

        private static Random __random = new Random();
        private static string __RandomString()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, __CARNUMBERLENGTH)
                .Select(s => s[__random.Next(s.Length)]).ToArray());
        }

        public ParkingRegisterControllerTest()
        {
            LotSizeInfo.SetupLots(__NUMBEROFSLOTS);
            __carNumbers = Enumerable.Range(0, __NUMBEROFCARS).Select(i => __RandomString()).ToArray();
            __controller = new ParkingRegisterController(new SlotRegistry(), new CarRegistry());
        }

        [Fact]
        public void Park1stCar()
        {
            var result = __controller.ParkCar(__carNumbers[0]);
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "1"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void Park5thCar()
        {
            int i = 0;
            while (i < 4)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.ParkCar(__carNumbers[4]);
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = "5"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void ParkCarInLastSlot()
        {
            int i = 0;
            while (i < __NUMBEROFSLOTS - 1)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.ParkCar(__carNumbers[__NUMBEROFSLOTS - 1]);
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                SlotNumber = $"{__NUMBEROFSLOTS}"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TryParkingCarWhenLotIsFull()
        {
            int i = 0;
            while (i < __NUMBEROFSLOTS)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.ParkCar(__carNumbers[__NUMBEROFSLOTS]);
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                ErrorMessage = "car park is full"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TryParkingCarWithoutNumber()
        {
            string expectedErrorMessage = "no valid car number given";
            var result = __controller.ParkCar(null);
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                ErrorMessage = expectedErrorMessage
            }), JsonSerializer.Serialize(result));
            result = __controller.ParkCar("");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                ErrorMessage = expectedErrorMessage
            }), JsonSerializer.Serialize(result));
            result = __controller.ParkCar("     ");
            Assert.IsType<ParkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new ParkCarResponse
            {
                ErrorMessage = expectedErrorMessage
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void UnparkCarFrom3rdSlot()
        {
            int i = 0;
            while (i < 4)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            __controller.ParkCar(__carNumbers[4]);
            var result = __controller.UnparkCar("3");
            Assert.IsType<UnparkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new UnparkCarResponse
            {
                Success = true
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void UnparkCarFrom5thSlotAfter3rdSlot()
        {
            int i = 0;
            while (i < 4)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            __controller.ParkCar(__carNumbers[4]);
            __controller.UnparkCar("3");
            var result = __controller.UnparkCar("5");
            Assert.IsType<UnparkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new UnparkCarResponse
            {
                Success = true
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void UnparkNonexistingCarFrom3rdAnd6thSlot()
        {
            int i = 0;
            while (i < 4)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            __controller.ParkCar(__carNumbers[4]);
            __controller.UnparkCar("3");
            new string[] { "3", "6" }.ToList().ForEach(slotNumber =>
            {
                var result = __controller.UnparkCar(slotNumber);
                Assert.IsType<UnparkCarResponse>(result);
                Assert.Equal(JsonSerializer.Serialize(new UnparkCarResponse
                {
                    Success = false,
                    ErrorMessage = $"slot {slotNumber} has no car"
                }), JsonSerializer.Serialize(result));
            });
        }

        [Fact]
        public void TryUnparkingCarFormNoneExistentSlot()
        {
            var result = __controller.UnparkCar($"{__NUMBEROFSLOTS + 2}");
            Assert.IsType<UnparkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new UnparkCarResponse
            {
                ErrorMessage = "no slot exists"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TryUnparkingCarWithoutNumber()
        {
            var result = __controller.UnparkCar(null);
            Assert.IsType<UnparkCarResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new UnparkCarResponse
            {
                ErrorMessage = "slot number is invalid"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void GetInfoWithSlotNumberWhereCarExists()
        {
            int i = 0;
            while (i <= 144)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.CarSlotInfo("77", null);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                SlotNumber = "77",
                CarNumber = __carNumbers[76]
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void GetInfoWithSlotNumberWhichIsEmpty()
        {
            int i = 0;
            while (i <= 144)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.CarSlotInfo("146", null);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                SlotNumber = "146"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void GetInfoWithNonexistentSlotNumber()
        {
            var result = __controller.CarSlotInfo($"{__NUMBEROFSLOTS + 3}", null);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                ErrorMessage = "no slot exists"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void GetInfoWithExistingCarNumber()
        {
            int i = 0;
            while (i <= 533)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            var result = __controller.CarSlotInfo(null, __carNumbers[532]);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                CarNumber = __carNumbers[532],
                SlotNumber = "533"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TryGettingInfoWithNonexistentCarNumber()
        {
            int i = 0;
            while (i <= 1001)
            {
                __controller.ParkCar(__carNumbers[i]);
                i++;
            }
            __controller.UnparkCar("354");
            var result = __controller.CarSlotInfo(null, __carNumbers[353]);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                ErrorMessage = "car not found"
            }), JsonSerializer.Serialize(result));
        }

        [Fact]
        public void TryGettingInfoWithoutCarAndSlotNumbers()
        {
            var result = __controller.CarSlotInfo(null, " ");
            Assert.IsType<CarSlotInfoResponse>(result);
            Assert.Equal(JsonSerializer.Serialize(new CarSlotInfoResponse
            {
                ErrorMessage = "car and slot numbers are invalid"
            }), JsonSerializer.Serialize(result));
        }
    }
}