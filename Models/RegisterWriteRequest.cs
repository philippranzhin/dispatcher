using DispatcherDesktop.Device.Data;

namespace DispatcherDesktop.Models
{
    public class RegisterWriteData
    {
        public RegisterWriteData(RegisterId id, uint integerAddress, uint? floatAddress, double value)
        {
            this.Id = id;
            this.IntegerAddress = integerAddress;
            this.FloatAddress = floatAddress;
            this.Value = value;
        }

        public RegisterId Id { get; }

        public uint IntegerAddress { get; }

        public uint? FloatAddress { get; }

        public double Value { get; }
    }
}
