namespace DispatcherDesktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RegisterDescription
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public uint IntegerAddress { get; set; }

        public uint? FloatAddress { get; set; }

        public uint? WriteAddress { get; set; }

        public string Postfix { get; set; }

        public static RegisterDescription Empty = new RegisterDescription();
    }
}
