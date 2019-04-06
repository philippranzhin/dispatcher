namespace DispatcherDesktop.Modbus
{
    using System.Collections.Generic;
    using System.Windows.Documents;

    using DispatcherDesktop.Models;

    public interface IDevicesConfigurationProvider
    {
        List<DeviceDescription> Devices { get; }
    }
}
