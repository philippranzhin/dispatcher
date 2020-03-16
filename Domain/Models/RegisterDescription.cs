namespace DispatcherDesktop.Domain.Models
{
    using System;

    public class RegisterDescription : ICloneable
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public uint IntegerAddress { get; set; }

        public uint? FloatAddress { get; set; } 

        public uint? WriteAddress { get; set; }

        public string Postfix { get; set; }

        public static RegisterDescription Empty = new RegisterDescription();
        
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
