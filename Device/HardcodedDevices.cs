namespace DispatcherDesktop.Device
{
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public class HardcodedDevices : IDevicesConfigurationProvider
    {
        public HardcodedDevices()
        {
            this.Devices = new List<DeviceDescription>()
                               {
                                   new DeviceDescription(1, "first"),
                                   new DeviceDescription(2, "second"),
                                   new DeviceDescription(3, "first"),
                                   new DeviceDescription(4, "second"),
                                   new DeviceDescription(5, "first"),
                                   new DeviceDescription(6, "second"),
                                   new DeviceDescription(7, "first"),
                                   new DeviceDescription(8, "second"),
                                   new DeviceDescription(1, "first"),
                                   new DeviceDescription(2, "second"),
                                   new DeviceDescription(3, "first"),
                                   new DeviceDescription(4, "second"),
                                   new DeviceDescription(5, "first"),
                                   new DeviceDescription(6, "second"),
                                   new DeviceDescription(7, "first"),
                                   new DeviceDescription(8, "second"),
                               };
        }

        public List<DeviceDescription> Devices { get; }
    }
}
