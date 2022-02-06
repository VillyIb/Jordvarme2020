﻿using System.Collections.Generic;

namespace eu.iamia.NCD.API.Contract
{
    /// <summary>
    /// Raw I2C Command Request.
    /// </summary>
    public interface ICommand
    {
        IEnumerable<byte> I2C_Data();

        I2CCommandCode GetI2CCommandCode { get; }
    }
}