namespace DispatcherDesktop.Device.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DispatcherDesktop.Configuration;
    using Configuration;
    using Data;
    using Models;

    public class SurveyService : ISurveyService
    {
        private readonly IDeviceIoDriver ioDriver;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly ISettingsProvider settingsProvider;

        private bool surveyStarted;

        private Queue<Tuple<RegisterWriteData, Action>> writeQueue;

        private object writeQueueSync = new object();

        private volatile bool surveyCompleted;
        private volatile bool paused;

        public SurveyService(IDeviceIoDriver ioDriver, IDevicesConfigurationProvider configurationProvider, ISettingsProvider settingsProvider)
        {
            this.ioDriver = ioDriver;
            this.configurationProvider = configurationProvider;
            this.settingsProvider = settingsProvider;
            this.writeQueue = new Queue<Tuple<RegisterWriteData, Action>>();

            this.surveyStarted = false;
            this.surveyCompleted = true;
            this.paused = false;
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
                    this.ReadAll();
                }

                this.surveyStarted = value;

                this.SurveyStartedChanged?.Invoke(this, value);
            }
        }

        public async void PauseOn(uint milliseconds)
        {
            this.paused = true;
            await Task.Delay((int) milliseconds);
            this.paused = false;
        }

        public void ScheduleWriteOperation(RegisterWriteData request, Action onSuccess)
        {
            lock (this.writeQueueSync)
            {
                this.writeQueue.Enqueue(new Tuple<RegisterWriteData, Action>(request, onSuccess));
            }
        }

        public event EventHandler<bool> SurveyStartedChanged;

        private async Task Read(DeviceDescription description)
        {
            if (this.paused)
            {
                return;
            }

			try
			{
				await this.ioDriver.Read(description);
			}
            catch
			{
				// ignore now, will add logger
			}
        }
        
        private void ReadAll(bool selfInvoked = false)
        {
            if (!selfInvoked && !this.surveyCompleted)
            {
                return;
            }

            this.surveyCompleted = false;

            lock (this.writeQueueSync)
            {
                var localWriteQueue = new Queue<Tuple<RegisterWriteData, Action>>();

                while (this.writeQueue.Count > 0)
                {
                    localWriteQueue.Enqueue(this.writeQueue.Dequeue());
                }

                Task.Run(async () =>
                {
                    while (localWriteQueue.Count > 0)
                    {
                        var currentRequest = localWriteQueue.Dequeue();
                        var result = await this.ioDriver.Write(currentRequest.Item1);

                        if (result)
                        {
                            currentRequest.Item2.Invoke();
                        }
                    }

                    foreach (var device in this.configurationProvider.Devices)
                    {
                        await Task.WhenAny(this.Read(device), Task.Delay(80000));
                    }

                    await Task.Delay(this.settingsProvider.SurveyPeriodSeconds * 1000);


                    if (this.surveyStarted)
                    {
                        this.ReadAll(true);
                    }
                    else
                    {
                        this.surveyCompleted = true;
                    }
                });
            }
        }
    }
}
