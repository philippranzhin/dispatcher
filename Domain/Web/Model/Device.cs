namespace DispatcherDesktop.Domain.Web.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using Utils;

    public struct Device
    {
        public Device(string address, List<Register> registers)
        {
            this.Address = address;
            this.Registers = registers;
        }

        public string Address { get; }
        
        public List<Register> Registers { get; }
        
        public override bool Equals(object obj)
        {
            if (obj is Device device)
            {
                return this.Address == device.Address 
                       && this.Registers.SequenceEqual(device.Registers);
            }

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 31 + this.Address.GetHashCode();
                hash = hash * 31 + this.Registers.GetSequenceHashCode();

                return hash;
            }
        }
    }
}