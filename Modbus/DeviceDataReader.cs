using System;

namespace DispatcherDesktop.Modbus
{
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    public class DeviceDataReader : IDeviceDataReader
    {
        public async Task<DeviceData> Read(DeviceDescription description)
        {
            return await Task.Run<DeviceData>(
                () => new DeviceData(description.Name, description.Id)
                          {
                              OutdoorTemperature = 5F,
                              DirectTemperature = 70.0F,
                              ReturnTemperature = 50.0F,
                              MixingTemperature = 60.0F,
                              GivingHeatingTemperature = 75.0F,
                              HeatingOpeningLimit = 90,
                              HeatingClosingLimit = 20,
                              Seazon = Seazon.Summer,

                          });
        }
    }
}
