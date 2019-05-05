

namespace DispatcherDesktop.Device
{
    using System;
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    public class DeviceDataReader : IDeviceDataReader
    {
        public async Task<bool> Read(DeviceDescription description)
        {
            return await Task.Run<bool>(
                       () =>
                           {
                               description.Registers.ForEach((r) => r.DataSliceDate = DateTime.Now);
                               return true;
                           });
        }
    }
}
