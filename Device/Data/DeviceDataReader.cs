﻿namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Threading.Tasks;

    using DispatcherDesktop.Configuration;
    using DispatcherDesktop.Device.Driver;
    using DispatcherDesktop.Models;

    using Modbus.Device;

    public class DeviceDataReader : IDeviceDataReader
    {
        private readonly IStorage storage;

        private readonly ISettingsProvider settings;

        public DeviceDataReader(IStorage storage, ISettingsProvider settings)
        {
            this.storage = storage;
            this.settings = settings;
        }

        public async Task Read(DeviceDescription description)
        {
            var port = new SerialPort(this.settings.ConnectionString)
                           {
                               BaudRate = 9600,
                               DataBits = 8,
                               Parity = Parity.None,
                               StopBits = StopBits.One
                           };

            if (!port.IsOpen)
            {
                try
                {
                    port.Open();
                }
                catch (Exception)
                {
                    return;
                }
            }

            
            IModbusSerialMaster master = ModbusSerialMaster.CreateRtu(new SerialAdapter(port));
            master.Transport.ReadTimeout = 3000;

            var requestQueue = this.SplitRegistersInfo(description.Registers);

            foreach (var requestGroup in requestQueue)
            {
                var startAddress = (ushort)requestGroup.First().IntegerAddress;

                var registersNumber = (int)(this.RegisterWeight(requestGroup.Last()) - startAddress + 1);

                try
                {
                    var result = await master.ReadHoldingRegistersAsync((byte)description.Id, startAddress, (ushort)registersNumber);

                    if (result != null && result.Any())
                    {
                        this.ParseResult(description, requestGroup, registersNumber, result);
                    }
                }
                catch
                {
                    // Ignore, will create logger later
                }
               
            }

            port.Close();
        }

        public List<List<RegisterDescription>> SplitRegistersInfo(List<RegisterDescription> registers)
        {
            var sorted = registers.OrderBy(this.RegisterWeight);

            var min = this.RegisterWeight(sorted.First());
            var i = 0;

            var requestQueue = new List<List<RegisterDescription>>() { new List<RegisterDescription>() };

            foreach (var registerInfo in sorted)
            {
                var current = this.RegisterWeight(registerInfo);

                if (current < (min + 120))
                {
                    requestQueue.ElementAt(i).Add(registerInfo);
                }
                else
                {
                    i++;
                    min = current;
                    requestQueue.Add(new List<RegisterDescription>() { registerInfo });
                }
            }


            return requestQueue;
        }

        private uint RegisterWeight(RegisterDescription r)
        {
            if (r.FloatAddress != null)
            {
                return (uint)r.FloatAddress;
            }

            return r.IntegerAddress;
        }

        private double ToFloat(ushort left, ushort right)
        {
            ushort[] data = new ushort[2] { left, right };
            float[] floatData = new float[1];
            Buffer.BlockCopy(data, 0, floatData, 0, data.Length * 2);
            return floatData[0];
        }

        public void ParseResult(DeviceDescription device, List<RegisterDescription> descriptions, int length, ushort[] data)
        {
            var startOffset = (int)descriptions.First().IntegerAddress;

            var i = 0;

            foreach (var register in descriptions)
            {
                i =  (int)register.IntegerAddress - startOffset;

                var count = this.RegisterLength(register);

                var bytes = data.Skip(i).Take(count).ToArray();

                if (bytes.Length == 2)
                {
                    var parsedData = new RegisterData(new RegisterId(device, register), DateTime.Now, this.ToFloat(bytes[0], bytes[1]));
                    this.storage.Save(parsedData);
                }
                else if (bytes.Length == 1)
                {
                    var parsedData = new RegisterData(new RegisterId(device, register), DateTime.Now, bytes[0]);
                    this.storage.Save(parsedData);
                }
                else
                {
                    return;
                }
            }
        }

        private int RegisterLength(RegisterDescription register)
        {
            return register.FloatAddress == null ? 1 : 2;
        }
    }
}