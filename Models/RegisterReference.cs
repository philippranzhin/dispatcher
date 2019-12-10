namespace DispatcherDesktop.Models
{
    public class RegisterReference
    {
        public RegisterReference(uint deviceId, RegisterDescription registerDescription)
        {
            this.RegisterDescription = registerDescription;
            this.DeviceId = deviceId;
        }

        public uint DeviceId { get; }

        public RegisterDescription RegisterDescription { get; }
    }
}
