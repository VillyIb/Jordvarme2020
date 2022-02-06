﻿using System;
using System.Collections.Generic;
using BoosterPumpLibrary.Settings;
using eu.iamia.i2c.communication.contract;
using eu.iamia.Util.Extensions;

namespace Modules.AMS5812
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once InconsistentNaming
    // see: https://store.ncd.io/product/ams5812-0150-d-b-amplified-pressure-sensor-1034-to-1034-mbar-15-to-15-psi-i2c-mini-module/

    public class AMS5812_0150_D_Pressure
    {
        private readonly IInputModule ComModule;

        public static byte DefaultAddressValue => 0x78;

        public  byte DefaultAddress => DefaultAddressValue;

        public virtual byte LengthRequested => 0x04;

        protected static uint DevicePressureMin => 3277;
        protected static uint DevicePressureMax => 29491;

        protected virtual float OutputPressureMin => -1034f;
        protected virtual float OutputPressureMax => 1034f;

        protected static uint DeviceTempMin => 3277;
        protected static uint DeviceTempMax => 29491;

        protected virtual float OutputTempMin => -25f;
        protected virtual float OutputTempMax => 85f;

        public float Pressure => Readings.IsInputDirty
            ? float.NaN
            : (float)Math.Round(
                (PressureHex.Value - DevicePressureMin) *
                (OutputPressureMax - OutputPressureMin) /
                (DevicePressureMax - DevicePressureMin) +
                OutputPressureMin,
                2);

        public float Temperature => Readings.IsInputDirty
            ? float.NaN
            : (float)Math.Round(
                (TemperatureHex.Value - DeviceTempMin) *
                (OutputTempMax - OutputTempMin) /
                (DeviceTempMax - DeviceTempMin) +
                OutputTempMin,
                2);

        private void Clear()
        {
            Readings.IsInputDirty = true;
        }

        #region Feature Setting 0xx78 (input)

        private readonly Register Readings = new(0x78, "Readings", 4, Direction.Input);

        private Int16BitSettingsWrapper TemperatureHex => new(Readings.GetOrCreateSubRegister(16, 0, "Temperature"));

        private Int16BitSettingsWrapper PressureHex => new(Readings.GetOrCreateSubRegister(16, 16, "Pressure"));

        #endregion

        protected  IEnumerable<Register> Registers => new[]
        {
            Readings
        };

        /// <summary>
        /// Pressure module
        /// </summary>
        public AMS5812_0150_D_Pressure( IInputModule comModule) //: base(apiToSerialBridge)
        {
            ComModule = comModule;
            ComModule.SetupOnlyOnce(Registers, DefaultAddressValue);
            Clear();
        }

        public  bool IsInputValid =>
            float.IsFinite(Temperature)
            &&
            Temperature.IsWithinRange(OutputTempMin, OutputTempMax)
            &&
            float.IsFinite(Pressure)
            && Pressure.IsWithinRange(OutputPressureMin, OutputPressureMax)
            ;

        public  void ReadFromDevice()
        {
            Clear();
            ComModule.ReadFromDevice();
        }
    }
}
