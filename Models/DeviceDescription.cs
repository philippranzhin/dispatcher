namespace DispatcherDesktop.Models
{
    using System.Collections.Generic;

    public class DeviceDescription
    {
        public DeviceDescription(uint id, string name, List<RegisterDescription> registers)
        {
            this.Id = id;
            this.Name = name;
            this.Registers = registers;
        }

        public uint Id { get ; }

        public string Name { get; }

        public List<RegisterDescription> Registers { get; }
    }
}
