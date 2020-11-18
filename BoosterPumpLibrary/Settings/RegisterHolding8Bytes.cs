﻿using System;

namespace BoosterPumpLibrary.Settings
{
    public class RegisterHolding1Byte : RegisterBase<byte>
    {
        protected override int MaxByteSize => 1;

        public RegisterHolding1Byte(byte registerIndex, string description, int byteCount) : base(registerIndex, description, byteCount)
        { }

        internal override void SetValue(ulong value)
        {
            Value = (byte)value;
        }

        internal override ulong GetValue()
        {
            return Value;
        }
    }

    public class RegisterHolding2Bytes : RegisterBase<ushort>
    {
        protected override int MaxByteSize => 2;

        public RegisterHolding2Bytes(byte registerIndex, string description, int byteCount) : base(registerIndex, description, byteCount)
        { }
      
        internal override void SetValue(ulong value)
        {
            Value = (ushort)value;
        }

        internal override ulong GetValue()
        {
            return Value;
        }
    }

    public class RegisterHolding4Bytes : RegisterBase<uint>
    {
        protected override int MaxByteSize => 4;

        public RegisterHolding4Bytes(byte registerIndex, string description, int byteCount) : base(registerIndex, description, byteCount)
        { }

        internal override void SetValue(ulong value)
        {
            Value = (uint)value;
        }

        internal override ulong GetValue()
        {
            return Value;
        }
    }

    public class RegisterHolding8Bytes : RegisterBase<ulong>
    {
        protected override int MaxByteSize => 8;

        public RegisterHolding8Bytes(byte registerIndex, string description, int byteCount) : base(registerIndex, description, byteCount)
        { }              

        public ulong GetDataRegisterAndClearDirty()
        {
            IsDirty = false;
            return Value;
        }

        internal override void SetValue(ulong value)
        {
            Value = value;
        }

        internal override ulong GetValue()
        {
            return Value;
        }
    }
}
