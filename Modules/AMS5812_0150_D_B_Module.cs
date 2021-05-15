﻿using System;
using System.Collections.Generic;
using BoosterPumpLibrary.ModuleBase;
using BoosterPumpLibrary.Settings;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;

namespace Modules
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    public class AMS5812_0150_D_B_Module : BaseModuleV2
    {
        public static byte DefaultAddressValue => 0x78;

        public override byte DefaultAddress => DefaultAddressValue;

        public override byte LengthRequested => 0x04;

        /// <summary>
        /// Pressure module
        /// </summary>
        /// <param name="apiToSerialBridge"></param>
        public AMS5812_0150_D_B_Module(IBridge apiToSerialBridge) : base(apiToSerialBridge)
        { }

        public int DevicePressureMin => 3277;
        public int DevicePressureMax => 29491;

        public virtual float OutputPressureMin => -1034f;
        public virtual float OutputPressureMax => 1034f;

        public int DeviceTempMin => 3277;
        public int DeviceTempMax => 29491;

        public virtual float OutputTempMin => -25f;
        public virtual float OutputTempMax => 85f;

        public float Pressure { get; protected set; }

        public float PressureCorrection { get; set; }

        public float Temperature { get; protected set; }

        protected override IEnumerable<RegisterBase> Registers => new List<RegisterBase>(0);

        public void ReadFromDevice()
        {
            var command = new CommandRead(DeviceAddress, LengthRequested);

            var response =  ApiToSerialBridge.Execute(command);

            if (!response.IsValid) { return; }

            var measuredPressure = response.Payload[0] << 8 | response.Payload[1];

            Pressure = (float)Math.Round(
                (measuredPressure - DevicePressureMin) *
                (OutputPressureMax - OutputPressureMin) /
                (DevicePressureMax - DevicePressureMin) +
                OutputPressureMin,
                2);

            var measuredTemp = response.Payload[2] << 8 | response.Payload[3];

            Temperature = (float)Math.Round(
                (measuredTemp - DeviceTempMin) *
                (OutputTempMax - OutputTempMin) /
                (DeviceTempMax - DeviceTempMin) +
                OutputTempMin,
                2);
        }
    }
}
