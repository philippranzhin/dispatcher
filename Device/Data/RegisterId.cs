namespace DispatcherDesktop.Device.Data
{
    using Models;

    public struct RegisterId
    {
        public RegisterId(uint device, RegisterDescription register)
        {
            this.Device = device;
            this.Register = register.IntegerAddress;
        }

        public uint Device { get; }

        public uint Register { get; }
    }
}
