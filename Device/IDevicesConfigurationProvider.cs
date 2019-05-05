﻿namespace DispatcherDesktop.Device
{
    using System;
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public interface IDevicesConfigurationProvider
    {
        ICollection<DeviceDescription> Devices { get; }

        void Save(ICollection<DeviceDescription> devices);

        void Save(DeviceDescription device);

        void Save();

        event EventHandler Saved;
    }
}
