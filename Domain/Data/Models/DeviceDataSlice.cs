namespace DispatcherDesktop.Domain.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DeviceDataSlice
    {
        public DeviceDataSlice(List<RegisterDataSlice> registerDataSlices)
        {
            if (registerDataSlices.Count <= 0)
            {
                throw new ArgumentException("empty slices collection not allowed");
            }

            var first = registerDataSlices.First();

            if (registerDataSlices.Any(r => r.Id.Device != first.Id.Device))
            {
                throw new ArgumentException("all data slices should be from one device");
            }
            
            this.RegisterDataSlices = registerDataSlices;
            this.DeviceId = first.Id.Device;
        }

        public uint DeviceId { get; }

        public List<RegisterDataSlice> RegisterDataSlices { get; }
    }
}