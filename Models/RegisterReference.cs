namespace DispatcherDesktop.Models
{
    public class RegisterReference
    {
        public RegisterReference(DeviceDescription device, RegisterDescription register)
        {
            this.Register = register;
            this.Device = device;
        }

        public DeviceDescription Device { get; }

        public RegisterDescription Register { get; }
    }
}
