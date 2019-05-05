namespace DispatcherDesktop.Device.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DispatcherDesktop.Models;

    using Newtonsoft.Json;

    public class DevicesConfigurationProvider : IDevicesConfigurationProvider
    {
        private ICollection<DeviceDescription> devices;

        public ICollection<DeviceDescription> Devices
        {
            get
            {
                try
                {
                    this.devices = JsonConvert.DeserializeObject<List<DeviceDescription>>(Properties.Settings.Default.Devices) ?? new List<DeviceDescription>();
                }
                catch (JsonSerializationException)
                {
                    this.devices = new List<DeviceDescription>();
                }
               

                return this.devices;
            }
        }

        public void Save(ICollection<DeviceDescription> devices)
        {
            var serializeObject = JsonConvert.SerializeObject(devices);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
            this.devices = devices;
        }

        public void Save(DeviceDescription device)
        {

            var changedDevices = this.devices.Select(
                (d) =>
                    {
                        if (d.Id == device.Id)
                        {
                            return device;
                        }

                        return d;
                    });

            this.devices = changedDevices.ToList();

            var serializeObject = JsonConvert.SerializeObject(this.devices);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
            this.Saved?.Invoke(this, EventArgs.Empty);
        }

        public void Save()
        {
            var serializeObject = JsonConvert.SerializeObject(this.devices);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
            this.Saved?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler Saved;
    }
}
