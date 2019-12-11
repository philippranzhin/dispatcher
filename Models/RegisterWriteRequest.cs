﻿using DispatcherDesktop.Device.Data;

namespace DispatcherDesktop.Models
{
    public class RegisterWriteData
    {
        public RegisterWriteData(RegisterId id, uint integerAddress, uint? floatAddress, uint writeAddress, double value)
        {
            this.Id = id;
            this.IntegerAddress = integerAddress;
            this.FloatAddress = floatAddress;
            this.Value = value;
            this.WriteAddress = writeAddress;
        }

        public RegisterId Id { get; }

        public uint IntegerAddress { get; }

        public uint? FloatAddress { get; }

        public uint WriteAddress { get; }

        public double Value { get; }
    }
}
