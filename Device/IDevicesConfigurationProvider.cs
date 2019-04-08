namespace DispatcherDesktop.Device
{
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public interface IDevicesConfigurationProvider
    {
        List<DeviceDescription> Devices { get; }
    }
}
