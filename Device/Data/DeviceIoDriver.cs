﻿namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO.Ports;
    using System.Linq;
    using System.Threading.Tasks;
    using Driver;
    using Models;

    using Modbus.Device;
    using Survey;

    public class DeviceIoDriver : IDeviceIoDriver
    {
        private readonly IStorage storage;

        private readonly SerialPort port;

        private readonly IModbusSerialMaster modbusMaster;

        public DeviceIoDriver(IStorage storage, ISurveySettingsProvider surveySettings)
        {
            this.storage = storage;

            this.port = new SerialPort(surveySettings.ConnectionString)
            {
                BaudRate = 9600,
                DataBits = 8,
                Parity = Parity.None,
                StopBits = StopBits.One
            };

            this.modbusMaster = ModbusSerialMaster.CreateRtu(new SerialAdapter(this.port));

            this.modbusMaster.Transport.ReadTimeout = 3000;
            this.modbusMaster.Transport.WriteTimeout = 3000;
        }

        private int i = 0;

        public async Task Read(DeviceDescription description)
        {
            this.OpenPort();

            var requestQueue = this.SplitRegistersInfo(description.Registers);

            foreach (var requestGroup in requestQueue)
            {
                var startAddress = (ushort)requestGroup.First().IntegerAddress;

                var registersNumber = (int)(this.RegisterWeight(requestGroup.Last()) - startAddress + 1);

                try
                {
                    var result = await this.modbusMaster.ReadHoldingRegistersAsync((byte)description.Id, startAddress, (ushort)registersNumber);
                    
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

            this.ClosePort();
        }

        public async Task<bool> Write(RegisterWriteData request)
        {
            try
            {
                this.OpenPort();
                if (request.FloatAddress == null)
                {
                    await this.modbusMaster.WriteSingleRegisterAsync(
                        (byte) request.Id.Device,
                        (ushort) request.WriteAddress,
                        (ushort) request.Value);
                }
                else
                {
                    await this.modbusMaster.WriteMultipleRegistersAsync(
                        (byte) request.Id.Device,
                        (ushort) request.WriteAddress,
                        this.ToUshort(request.Value));
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                this.ClosePort();
            }
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

        private ushort[] ToUshort(double value)
        {
            var uintData = new ushort[2];
            var floatData = new float[1] { (float)value };

            Buffer.BlockCopy(floatData, 0, uintData, 0, 4);
            
            return uintData;
        }

        public void ParseResult(DeviceDescription device, List<RegisterDescription> descriptions, int length, ushort[] data)
        {
            var startOffset = (int)descriptions.First().IntegerAddress;

            foreach (var register in descriptions)
            {
                var i = (int)register.IntegerAddress - startOffset;

                var count = this.RegisterLength(register);

                var bytes = data.Skip(i).Take(count).ToArray();

                switch (bytes.Length)
                {
                    case 2:
                    {
                        var parsedData = new RegisterData(new RegisterId(device.Id, register), DateTime.Now, this.ToFloat(bytes[0], bytes[1]));
                        this.storage.Save(parsedData);
                        break;
                    }

                    case 1:
                    {
                        var parsedData = new RegisterData(new RegisterId(device.Id, register), DateTime.Now, bytes[0]);
                        this.storage.Save(parsedData);
                        break;
                    }

                    default:
                        return;
                }
            }
        }

        private int RegisterLength(RegisterDescription register)
        {
            return register.FloatAddress == null ? 1 : 2;
        }

        private void OpenPort()
        {
            if (!this.port.IsOpen)
            {
                try
                {
                    this.port.Open();
                }
                catch
                {
                    // ignored
                }
            }
        }

        private void ClosePort()
        {
            if (this.port.IsOpen)
            {
                try
                {
                    this.port.Close();
                }
                catch
                {
                    // ignore
                }
            }
        }
    }
}
