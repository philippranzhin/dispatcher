

namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    public class DeviceDataReader : IDeviceDataReader
    {
        private readonly IStorage storage;

        public DeviceDataReader(IStorage storage)
        {
            this.storage = storage;
        }

        public async Task Read(DeviceDescription description)
        {
            await Task.Run(
                () =>
                    {
                        description.Registers.ForEach((r) =>
                            {
                                var data = new RegisterData(new RegisterId(description, r), DateTime.Now, 0);
                                this.storage.Save(data);
                            });
                    });
        }
    }
}
