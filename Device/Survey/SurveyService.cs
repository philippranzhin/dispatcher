namespace DispatcherDesktop.Device.Survey
{
    using System;
    using System.Threading.Tasks;
    using System.Timers;

    using DispatcherDesktop.Configuration;
    using DispatcherDesktop.Device.Configuration;
    using DispatcherDesktop.Device.Data;
    using DispatcherDesktop.Models;

    class SurveyService : ISurveyService, IDisposable
    {
        private readonly IDeviceDataReader dataReader;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly ISettingsProvider settingsProvider;

        private readonly Timer lifeTimer;

        private bool surveyStarted;

        private volatile bool readCompleted;

        public SurveyService(IDeviceDataReader dataReader, IDevicesConfigurationProvider configurationProvider, ISettingsProvider settingsProvider)
        {
            this.dataReader = dataReader;
            this.configurationProvider = configurationProvider;
            this.settingsProvider = settingsProvider;

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

        public event EventHandler<bool> ServeyStartedChanged;

        private async Task Read(DeviceDescription description)
        {
            await this.dataReader.Read(description);
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
