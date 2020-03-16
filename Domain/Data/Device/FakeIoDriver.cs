namespace DispatcherDesktop.Domain.Data.Device
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Domain.Models;
    using Models;
    using Storage;

    public class FakeIoDriver : IDeviceIoDriver
    {
        private readonly Dictionary<Tuple<uint, uint>, double> cache = new Dictionary<Tuple<uint, uint>, double>();

        public async Task<List<RegisterDataSlice>> Read(DeviceDescription description)
        {
            await Task.Delay(1000);

            if (description.Id == 2)
            {
                throw new Exception();
            }
            
            return description.Registers.Select(r =>
            {
                var rand = new Random();

                var intV = (double)rand.Next(10, 100);
                var floatV = (double)rand.Next(10, 99);
                
                var value = 2.0 * r.IntegerAddress * description.Id;

                if (this.cache.TryGetValue(new Tuple<uint, uint>(description.Id, r.IntegerAddress), out var cachedValue))
                {
                    value = cachedValue;
                }
                
                var data = new RegisterDataSlice(new RegisterId(description.Id, r), DateTime.Now, value);
                
                return data;
            }).ToList();
        }

        public async Task<bool> Write(RegisterWriteRequest request)
        {
            await Task.Delay(1000);
            this.cache.Remove(new Tuple<uint, uint>(request.Id.Device, request.IntegerAddress));
            this.cache.Add(new Tuple<uint, uint>(request.Id.Device, request.IntegerAddress), request.Value);
            return true;
        }
    }
}