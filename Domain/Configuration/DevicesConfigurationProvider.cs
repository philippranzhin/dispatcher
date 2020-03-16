namespace DispatcherDesktop.Domain.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Models;
    using Models;
    using Newtonsoft.Json;

    public class DevicesConfigurationProvider : IDevicesConfigurationProvider
    {
        private readonly DeviceConfig config;

        public DevicesConfigurationProvider()
        {
            try
            {
                this.config = JsonConvert.DeserializeObject<DeviceConfig>(Properties.Settings.Default.Devices)
                              ?? new DeviceConfig();
            }
            catch (JsonSerializationException)
            {
                this.config = new DeviceConfig();
            }
        }

        public ICollection<DeviceDescription> Devices => this.config.Devices;

        public ICollection<DeviceMapper> Mappers => this.config.Mappers;

        public void SaveDevices(ICollection<DeviceDescription> devices)
        {
            this.config.Devices = devices;
            var serializeObject = JsonConvert.SerializeObject(this.config);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
            this.config.Devices = devices;
        }

        public void AddDevice(DeviceDescription device)
        {

            var changedDevices = this.config.Devices.Select(
                (d) =>
                    {
                        if (d.Id == device.Id)
                        {
                            return device;
                        }

                        return d;
                    });

            this.config.Devices = changedDevices.ToList();

            var serializeObject = JsonConvert.SerializeObject(this.config);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
            this.Saved?.Invoke(this, EventArgs.Empty);
        }

        public void AddMapper(DeviceMapper mapper)
        {
            this.config.Mappers.Add(mapper);
            var serializeObject = JsonConvert.SerializeObject(this.config);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
        }

        public void RemoveMapper(DeviceMapper mapper)
        {
            this.config.Mappers.Remove(mapper);
            var serializeObject = JsonConvert.SerializeObject(this.config);
            Properties.Settings.Default.Devices = serializeObject;
            Properties.Settings.Default.Save();
        }

        public event EventHandler Saved;
    }
}
