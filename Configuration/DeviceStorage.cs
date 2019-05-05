namespace DispatcherDesktop.Configuration
{
    using System.Collections.Generic;
    using System.Windows.Documents;

    using DispatcherDesktop.Models;

    public class DeviceStorage : IDeviceStorage
    {
        public void Save(List<DeviceDescription> devices)
        {

        }

        public List<DeviceDescription> Read()
        {
            return new List<DeviceDescription>();
        }
    }
}
