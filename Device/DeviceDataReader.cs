

namespace DispatcherDesktop.Device
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    using Modbus.Net;
    using Modbus.Net.Modbus;

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
                              Date = DateTime.Now,
                          });
        }

        private void CreateAddresses(DeviceDescription description)
        {
            List<AddressUnit> addressUnits = new List<AddressUnit>
                                                 {
                                                     new AddressUnit() {Id = "d0", Name="Variable 1", Area = "4X", Address = 1, CommunicationTag = "D1", DataType = typeof (ushort), Zoom = 0.01, DecimalPos = 2},
                                                     new AddressUnit() {Id = "d1", Name="Variable 2", Area = "4X", Address = 2, SubAddress = 0, CommunicationTag = "D2", DataType = typeof (bool)},
                                                     new AddressUnit() {Id = "d2", Name="Variable 3", Area = "4X", Address = 2, SubAddress = 1, CommunicationTag = "D3", DataType = typeof (bool)},
                                                     new AddressUnit() {Id = "d3", Name="Variable 4", Area = "4X", Address = 2, SubAddress = 8, CommunicationTag = "D4", DataType = typeof (byte)},
                                                     new AddressUnit() {Id = "d4", Name="Variable 5", Area = "4X", Address = 3, CommunicationTag = "D5", DataType = typeof (int)},
                                                     new AddressUnit() {Id = "d5", Name="Variable 6", Area = "4X", Address = 5, CommunicationTag = "D6", Zoom=0.1, DecimalPos = 1, DataType = typeof (float)},
                                                     new AddressUnit() {Id = "d6", Name="Variable 7", Area = "4X", Address = 7, CommunicationTag = "D7", DecimalPos = 1, DataType = typeof (float)},
                                                     new AddressUnit() {Id = "d7", Name="Variable 8", Area = "4X", Address = 9, CommunicationTag = "D8", DataType = typeof (short)},
                                                 };

            var t = new ModbusMachine(ModbusType.Rtu, "", addressUnits, false, (byte)description.Id, (byte)description.Id);
        }
    }
}
