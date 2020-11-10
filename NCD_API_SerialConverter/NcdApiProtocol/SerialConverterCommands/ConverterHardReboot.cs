﻿using NCD_API_SerialConverter.NcdApiProtocol.SerialConverterCommands;

namespace NCD_API_SerialConverter.Commands
{
    public class ConverterHardReboot : ConverterCommandBase
    {
        public override byte[] Payload => new byte[] { 0x21, 0xBD };
    }
}
