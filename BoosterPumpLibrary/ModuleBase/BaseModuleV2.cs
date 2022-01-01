﻿using System;
using System.Collections.Generic;
using System.Linq;
using BoosterPumpLibrary.Settings;
using EnsureThat;
using eu.iamia.NCD.API;
using eu.iamia.NCD.API.Contract;
using eu.iamia.Util.Extensions;

// ReSharper disable UnusedMember.Global

namespace BoosterPumpLibrary.ModuleBase
{
    public abstract partial class BaseModuleV2
    {
        protected readonly IBridge ApiToSerialBridge;

        public Guid Id { get; }

        public abstract byte DefaultAddress { get; }

        public ByteExtension AddressIncrement { get; protected set; }

        public byte DeviceAddress => DefaultAddress + (AddressIncrement ?? new ByteExtension(0));

        protected BaseModuleV2(IBridge apiToSerialBridge)
        {
            Ensure.That(apiToSerialBridge, nameof(apiToSerialBridge)).IsNotNull();

            ApiToSerialBridge = apiToSerialBridge;
            AddressIncrement = null;
            Id = Guid.NewGuid();
            Console.WriteLine($"{GetType().Name}: {Id}");
        }

        // TODO NOT generic - valid value range is independent for each module.
        /// <summary>
        /// Adds the specified value to the DefaultAddress, legal values: {0|1}.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetAddressIncrement(int value)
        {
            if (value.IsOutsideRange(0, 1)) { throw new ArgumentOutOfRangeException(nameof(value), value, "Valid: {0:1}"); }
            AddressIncrement = value;
        }

        protected abstract IEnumerable<Register> Registers { get; }

        public ModuleEnumerator GetEnumerator()
        {
            var registersToSend = Registers.Where(t => t.IsOutputDirty);
            return new(registersToSend, DeviceAddress);
        }

        public void SendOld()
        {
            //using var enumerator = GetEnumerator();
            //var retryCount = 0;
            //while (enumerator.MoveNext())
            //{
            //    var commandController = enumerator.Current;
            //    var fromDevice = SerialPort.Execute(commandController);

            //    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            //    if (retryCount > 0 && (fromDevice.Payload.Length != 1 || fromDevice.Payload[0] != 55))
            //    {
            //        enumerator.Reset();
            //        retryCount--;
            //    }
            //}
        }

        public void Send()
        {
            using var enumerator = GetEnumerator();
            var retryCount = 0;
            while (enumerator.MoveNext() && enumerator.Current != null)
            {
                var fromDevice = ApiToSerialBridge.Execute(enumerator.Current);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (retryCount <= 0 || fromDevice.Payload.Count == 1 && fromDevice.Payload[0] == 55) continue;
                enumerator.Reset();
                retryCount--;
            }
        }

        public void SelectRegisterForReadingOld(Register register)
        {
            //var writeCommand = new CommandWrite(DeviceAddress, new[] {register.RegisterAddress});
            //// ReSharper disable once UnusedVariable
            //var returnValue = SerialPort.Execute(writeCommand);
        }

        public void SelectRegisterForReadingWithAutoIncrementOld(Register register)
        {
            //var writeCommand = new CommandWrite(DeviceAddress, new[] {(byte) (register.RegisterAddress | 0x80)});
            //// ReSharper disable once UnusedVariable
            //var returnValue = SerialPort.Execute(writeCommand);
        }

        public void SelectRegisterForReading(Register register)
        {
            var writeCommand = new CommandWrite(DeviceAddress, new[] { register.RegisterAddress });
            // ReSharper disable once UnusedVariable
            var returnValue = ApiToSerialBridge.Execute(writeCommand);
        }

        public void SelectRegisterForReadingWithAutoIncrement(Register register)
        {
            var writeCommand = new CommandWrite(DeviceAddress, new[] { (byte)(register.RegisterAddress | 0x80) });
            // ReSharper disable once UnusedVariable
            var returnValue = ApiToSerialBridge.Execute(writeCommand);
        }

    }
}
