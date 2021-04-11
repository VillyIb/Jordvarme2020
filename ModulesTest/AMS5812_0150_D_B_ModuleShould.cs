﻿//using BoosterPumpLibrary.Commands;
//using BoosterPumpLibrary.Contracts;
using eu.iamia.NCD.DeviceCommunication.Contract;
using NSubstitute;
using Xunit;
using Modules;
//using IDataFromDevice = BoosterPumpLibrary.Contracts.IDataFromDevice;
using eu.iamia.NCD.API;

// ReSharper disable InconsistentNaming

namespace ModulesTest
{
    public class AMS5812_0150_D_B_ModuleShould
    {
        private readonly AMS5812_0150_D_B_Module _Sut;
        private readonly IGateway _FakeGateway;

        public AMS5812_0150_D_B_ModuleShould()
        {
            _FakeGateway = Substitute.For<IGateway>();
            _Sut = new AMS5812_0150_D_B_Module(_FakeGateway);
        }

        [Fact]
        public void SendReadSequenceCallingReadFromDevice()
        {
            //IDataFromDevice fakeReturnValue = new DataFromDevice { Header = 0xAA, ByteCount = 0x04, Payload = new byte[] { 0x3F, 0xEB, 0x36, 0xE2 }, Checksum = 0xF0 };
            //_FakeGateway.Execute(Arg.Any<ReadCommand>()).Returns(fakeReturnValue);

            //_FakeGateway.Execute(new ReadCommand(00, 0));

            _Sut.ReadFromDevice();

            _FakeGateway.Received().Execute(Arg.Is<ReadCommand>(c => c.I2CDataAsHex == "78 04 "));

            Assert.Equal(-1.66f, _Sut.Pressure);
            Assert.Equal(20.21f, _Sut.Temperature);
        }
    }
}
