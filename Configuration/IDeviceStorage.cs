namespace DispatcherDesktop.Configuration
{
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public interface IDeviceStorage
    {
        List<DeviceDescription> Read();

        void Save(List<DeviceDescription> devices);
    }
}