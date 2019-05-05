namespace DispatcherDesktop.Device.Data
{
    using DispatcherDesktop.Models;

    public struct RegisterId
    {
        public RegisterId(DeviceDescription device, RegisterDescription register)
        {
            this.Device = device.Id;
            this.Register = register.IntegerAddress;
        }

        public uint Device { get; }

        public uint Register { get; }
    }
}
