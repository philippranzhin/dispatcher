namespace DispatcherDesktop.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class RegisterDescription
    {
        public RegisterDescription()
        {
            this.Data = new List<int>();
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public uint IntegerAddress { get; set; }

        public uint? FloatAddress { get; set; }

        public List<int> Data { get; set; }

        public int? LatestData => this.Data.Last();

        public string Postfix { get; set; }

        public DateTime? DataSliceDate { get; set; }
    }
}
