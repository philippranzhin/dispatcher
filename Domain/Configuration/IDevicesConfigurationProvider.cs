namespace DispatcherDesktop.Domain.Configuration
{
    using System;
    using System.Collections.Generic;
    using Domain.Models;

    public interface IDevicesConfigurationProvider
    {
        ICollection<DeviceDescription> Devices { get; }
           
        ICollection<DeviceMapper> Mappers { get; }

        void SaveDevices(ICollection<DeviceDescription> devices);

        void AddDevice(DeviceDescription device);

        void AddMapper(DeviceMapper mapper);

        void RemoveMapper(DeviceMapper mapper);
        
        event EventHandler Saved;
    }
}
