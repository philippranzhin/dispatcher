namespace DispatcherDesktop.Device
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using System.Timers;

    using DispatcherDesktop.Configuration;
    using DispatcherDesktop.Models;
    using DispatcherDesktop.Properties;

    class DeviceDataProvider : IDeviceDataProvider, IDisposable
    {
        private readonly IDeviceDataReader dataReader;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly ISettingsProvider settingsProvider;

        private readonly Dictionary<int, DeviceData> recentData;

        private readonly Timer lifeTimer;

        private bool surveyStarted;

        private volatile bool readCompleted;

        public DeviceDataProvider(IDeviceDataReader dataReader, IDevicesConfigurationProvider configurationProvider, ISettingsProvider settingsProvider)
        {
            this.dataReader = dataReader;
            this.configurationProvider = configurationProvider;
            this.settingsProvider = settingsProvider;

            this.recentData = new Dictionary<int, DeviceData>();
            this.surveyStarted = false;

            this.lifeTimer = new Timer(this.settingsProvider.SurveyPeriodSeconds * 1000)
                                 {
                                     AutoReset = true,
                                 };

            this.readCompleted = true;

            this.ReadAll();

            this.lifeTimer.Elapsed += (s, e) => this.ReadAll();
        }

        public bool SurveyStarted
        {
            get => this.surveyStarted;
            set
            {
                if (this.surveyStarted == value)
                {
                    return;
                }

                if (value)
                {
                    this.lifeTimer.Start();
                }
                else
                {
                    this.lifeTimer.Stop();
                }

                this.surveyStarted = value;

                this.ServeyStartedChanged?.Invoke(this, value);
            }
        }

        public Dictionary<int, DeviceData> RecentData => this.recentData;

        public event EventHandler<DeviceData> DataReceived;

        public event EventHandler<bool> ServeyStartedChanged;

        private async Task Read(DeviceDescription description)
        {
            var receivedData = await this.dataReader.Read(description);
            if (receivedData != null)
            {
                if (this.recentData.ContainsKey(receivedData.Address))
                {
                    this.recentData.Remove(receivedData.Address);
                }

                this.recentData.Add(receivedData.Address, receivedData);

                this.DataReceived?.Invoke(this, receivedData);
            }
        }
         
        private void ReadAll()
        {
            if (!this.readCompleted)
            {
                return;
            }

            this.readCompleted = false;
            Task.Run(async () =>
                {
                    foreach (var device in this.configurationProvider.Devices)
                    {
                       await this.Read(device);
                    }

                    this.readCompleted = true;
                });
        }

        public void Dispose()
        {
            this.lifeTimer?.Dispose();
        }
    }
}
