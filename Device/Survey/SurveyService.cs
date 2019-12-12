namespace DispatcherDesktop.Device.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using DispatcherDesktop.Configuration;
    using Configuration;
    using Data;
    using Logger;
    using Models;

    public class SurveyService : ISurveyService
    {
        private readonly IDeviceIoDriver ioDriver;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly ISettingsProvider settingsProvider;

        private readonly ILogger logger;

        private bool surveyStarted;

        private readonly Queue<Tuple<RegisterWriteData, Action<bool>>> writeQueue;

        private readonly object writeQueueSync = new object();

        private volatile bool surveyCompleted;
        private volatile bool writeOperationRequested;

        public SurveyService(
            IDeviceIoDriver ioDriver, 
            IDevicesConfigurationProvider configurationProvider, 
            ISettingsProvider settingsProvider, 
            ILogger logger)
        {
            this.ioDriver = ioDriver;
            this.configurationProvider = configurationProvider;
            this.settingsProvider = settingsProvider;
            this.logger = logger;
            this.writeQueue = new Queue<Tuple<RegisterWriteData, Action<bool>>>();

            this.surveyStarted = false;
            this.surveyCompleted = true;
            this.writeOperationRequested = false;
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

        public void ScheduleWriteOperation(RegisterWriteData request, Action<bool> onFinish)
        {
            this.writeOperationRequested = true;
            lock (this.writeQueueSync)
            {
                this.writeQueue.Enqueue(new Tuple<RegisterWriteData, Action<bool>>(request, onFinish));
            }
        }

        public event EventHandler<bool> SurveyStartedChanged;

        private async Task Read(DeviceDescription description)
        {
            try
            {
                this.logger.LogInfo(Properties.Resources.LogLineSeparator);
                this.logger.LogInfo($"{Properties.Resources.ReadOperationStartLogMsg} {description.Name}");

                await this.ioDriver.Read(description);

                this.logger.LogInfo($"{Properties.Resources.ReadOperationSuccesLogMsg}");
            }
            catch (Exception e)
            {
                this.logger.LogError($"{Properties.Resources.ReadOperationFailureLogMsg}:");
                this.logger.LogError(e.Message);
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
                var localWriteQueue = new Queue<Tuple<RegisterWriteData, Action<bool>>>();

                while (this.writeQueue.Count > 0)
                {
                    localWriteQueue.Enqueue(this.writeQueue.Dequeue());
                }

                Task.Run(async () =>
                {
                    while (localWriteQueue.Count > 0)
                    {
                        var currentRequest = localWriteQueue.Dequeue();
                        var writeTask = this.Write(currentRequest.Item1);

                        var result = await Task.WhenAny(writeTask, Task.Delay(10000));

                        if (result == writeTask)
                        {
                            if (result.IsCompleted && result.Exception == null)
                            {
                                currentRequest.Item2.Invoke(writeTask.Result);

                                if (writeTask.Result)
                                {
                                    this.logger.LogInfo(Properties.Resources.WriteOperationSuccesLogMsg);
                                }
                                else
                                {
                                    this.logger.LogError(Properties.Resources.WriteOperationFailureLogMsg);
                                }
                            }
                            else
                            {
                                this.logger.LogError($"{Properties.Resources.WriteOperationFailureLogMsg}:");
                                this.logger.LogError(result.Exception?.Message);

                                currentRequest.Item2.Invoke(false);
                            }
                        }
                        else
                        {
                            currentRequest.Item2.Invoke(false);
                            this.logger.LogError(Properties.Resources.WriteOperationFailureByTimeoutLogMsg);
                        }
                    }

                    foreach (var device in this.configurationProvider.Devices)
                    {
                        var delayTask = Task.Delay(10000);

                        var result = await Task.WhenAny(this.Read(device), delayTask);

                        if (result == delayTask)
                        {
                            this.logger.LogError(Properties.Resources.ReadOperationFailureByTimeoutLogMsg);
                        }

                        if (this.writeOperationRequested)
                        {
                            break;
                        }
                    }

                    if (!this.writeOperationRequested)
                    {
                        await Task.Delay(this.settingsProvider.SurveyPeriodSeconds * 1000);
                    }

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

        private async Task<bool> Write(RegisterWriteData data)
        {
            this.logger.LogInfo(Properties.Resources.LogLineSeparator);
            this.logger.LogInfo($"{Properties.Resources.WriteOperationStartLogMsg} {data.Id.Device}");
            this.logger.LogInfo($"{Properties.Resources.WriteOperationRegister}: {data.Id.Register}");
            this.logger.LogInfo($"{Properties.Resources.WriteOperationValue}: {data.Value}");

            this.writeOperationRequested = false;

            return await this.ioDriver.Write(data);
        }
    }
}
