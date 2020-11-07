﻿using BoosterPumpLibrary.SerialConverter;

using System.Collections.Generic;
using System.Linq;

namespace BoosterPumpLibrary.SerialConverter
{
    using Commands;

    public class NCD_API_Packet_Write_Command : NCD_API_Packet_Command_Base<WriteCommand>
    {
        public NCD_API_Packet_Write_Command(WriteCommand backingValue) : base(backingValue)
        { }

        public override byte Length => (byte)(Payload.Count() + 2);

        public override byte Command => 0xBE;

        public byte[] Payload => BackingValue.Payload.ToArray();

        public override IEnumerable<byte> CommandData()
        {
            yield return Command;
            yield return Address ?? 0x00;
            foreach (var current in Payload)
            {
                yield return current;
            }
        }
    }
}
