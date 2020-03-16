namespace DispatcherDesktop.Domain.Data.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Configuration;
    using Device;
    using Domain.Models;
    using Logger;
    using Models;

    public class  SurveyService : ISurveyService
    {
        private readonly IDeviceIoDriver ioDriver;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly ISurveySettingsProvider surveySettingsProvider;

        private readonly ILogger logger;

        private bool surveyStarted;

        private readonly Queue<Tuple<RegisterWriteRequest, Action<bool>>> writeQueue;

        private readonly object writeQueueSync = new object();
        
        private readonly ManualResetEvent forceSurveyEvent;
        
        private volatile bool surveyCompleted;
        private volatile bool writeOperationRequested;



        public SurveyService(
            IDeviceIoDriver ioDriver, 
            IDevicesConfigurationProvider configurationProvider, 
            ISurveySettingsProvider surveySettingsProvider, 
            ILogger logger)
        {
            this.ioDriver = ioDriver;
            this.configurationProvider = configurationProvider;
            this.surveySettingsProvider = surveySettingsProvider;
            this.logger = logger;
            this.writeQueue = new Queue<Tuple<RegisterWriteRequest, Action<bool>>>();

            this.surveyStarted = false;
            this.surveyCompleted = true;
            this.writeOperationRequested = false;
            this.forceSurveyEvent = new ManualResetEvent(false);
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
                    this.Survey();
                }

                this.surveyStarted = value;

                this.SurveyStartedChanged?.Invoke(this, value);
            }
        }

        public void ScheduleWriteOperation(RegisterWriteRequest request, Action<bool> onFinish)
        {
            var registerExists = this
                .configurationProvider.Devices
                .FirstOrDefault(d => d.Id == request.Id.Device)
                ?.Registers
                .Any(r => r.IntegerAddress == request.IntegerAddress);
            
            if(registerExists != true) return;
            
            this.writeOperationRequested = true;
            lock (this.writeQueueSync)
            {
                this.ForceSurvey();
                this.writeQueue.Enqueue(new Tuple<RegisterWriteRequest, Action<bool>>(request, onFinish));
            }
        }

        public void ForceSurvey()
        {
            this.forceSurveyEvent.Set();
        }

        public event EventHandler<bool> SurveyStartedChanged;
        
        public event Action<DeviceDataSlice> DeviceDataReceived;
        
        private void Survey(bool selfInvoked = false)
        {
            if (!selfInvoked && !this.surveyCompleted)
            {
                return;
            }
            
            this.surveyCompleted = false;

            lock (this.writeQueueSync)
            {
                var localWriteQueue = new Queue<Tuple<RegisterWriteRequest, Action<bool>>>(this.writeQueue);
                this.writeQueue.Clear();
                
                Task.Run(async () =>
                {
                    var modifiedDevices = await this.WriteAll(localWriteQueue);

                    var sortedDevices = this.ReorderDevices(modifiedDevices);
                    
                    await this.ReadAll(sortedDevices);

                    await this.WaitSurveyPeriod();

                    if (this.surveyStarted)
                    {
                        this.Survey(true);
                    }
                    else
                    {
                        this.surveyCompleted = true;
                    }
                });
            }
        }

        private async Task ReadAll(IEnumerable<DeviceDescription> sortedDevices)
        {
            foreach (var device in sortedDevices)
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
        }

        private async Task WaitSurveyPeriod()
        {
            if (!this.writeOperationRequested)
            {
                var waitForceTask = Task.Run(() =>
                {
                    this.forceSurveyEvent.WaitOne();
                    this.forceSurveyEvent.Reset();
                });
                var delayTask = Task.Delay(this.surveySettingsProvider.SurveyPeriodSeconds * 1000);
                await Task.WhenAny(waitForceTask, delayTask);
            }
        }

        private async Task<List<uint>> WriteAll(Queue<Tuple<RegisterWriteRequest, Action<bool>>> localWriteQueue)
        {
            var modifiedDevices = new List<uint>();

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

                        modifiedDevices.Add(currentRequest.Item1.Id.Device);

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

            return modifiedDevices;
        }

        private IEnumerable<DeviceDescription> ReorderDevices(List<uint> modifiedDevices)
        {
            var devices = this.configurationProvider.Devices.ToList();

            if (modifiedDevices.Any())
            {
                devices.Sort((d1, d2) =>
                {
                    if (modifiedDevices.Any(md => md == d1.Id)) return -1;

                    if (modifiedDevices.Any(md => md == d2.Id)) return 1;
                    
                    return 0;
                });
            }
            
            return devices;
        }

        private async Task Read(DeviceDescription description)
        {
            try
            {
                if (description.Registers.Count == 0)
                {
                    return;
                }
                
                var mapper = this
                    .configurationProvider
                    .Mappers
                    .FirstOrDefault(m => m.Recipient == description.Id);

                if (mapper != null)
                {
                    await this.ReadFakeData(description, mapper);
                }
                else
                {
                    this.logger.LogInfo(Properties.Resources.LogLineSeparator);
                    this.logger.LogInfo($"{Properties.Resources.ReadOperationStartLogMsg} {description.Name}");
                
                    var data = await this.ioDriver.Read(description);

                    this.DeviceDataReceived?.Invoke(new DeviceDataSlice(data));
                
                    this.logger.LogInfo($"{Properties.Resources.ReadOperationSuccesLogMsg}");
                }
            }
            catch (Exception e)
            {
                this.logger.LogError($"{Properties.Resources.ReadOperationFailureLogMsg}:");
                this.logger.LogError(e.Message);
            }
        }

        private async Task ReadFakeData(DeviceDescription description, DeviceMapper mapper)
        {
            var descriptionToRead = this.configurationProvider.Devices.First(d => d.Id == mapper.Source);

            this.logger.LogInfo(Properties.Resources.LogLineSeparator);
            this.logger.LogInfo($"{Properties.Resources.ReadOperationStartLogMsg} {description.Name}");

            var fakedData =
                (await this.ioDriver.Read(descriptionToRead))
                .Where(slice => descriptionToRead.Registers.Any(r => r.IntegerAddress == slice.Id.Register))
                .Select(slice => new RegisterDataSlice(
                    new RegisterId(mapper.Recipient,
                        descriptionToRead.Registers.First(r => r.IntegerAddress == slice.Id.Register)),
                    slice.SaveDate,
                    slice.Value
                ))
                .ToList();

            this.DeviceDataReceived?.Invoke(new DeviceDataSlice(fakedData));

            this.logger.LogInfo($"{Properties.Resources.ReadOperationSuccesLogMsg}");
        }

        private async Task<bool> Write(RegisterWriteRequest request)
        {
            this.logger.LogInfo(Properties.Resources.LogLineSeparator);
            this.logger.LogInfo($"{Properties.Resources.WriteOperationStartLogMsg} {request.Id.Device}");
            this.logger.LogInfo($"{Properties.Resources.WriteOperationRegister}: {request.Id.Register}");
            this.logger.LogInfo($"{Properties.Resources.WriteOperationValue}: {request.Value}");

            this.writeOperationRequested = false;

                            
            var mapper = this
                .configurationProvider
                .Mappers
                .FirstOrDefault(m => m.Recipient == request.Id.Device);

            if (mapper != null)
            {
                var register = this
                    .configurationProvider
                    .Devices
                    ?.First(d => d.Id == mapper.Source)
                    ?.Registers.First(r => r.IntegerAddress == request.IntegerAddress);

                if (register?.WriteAddress == null) return false;
                
                var registerId = new RegisterId(mapper.Source, register);
                
                var fakeRequest = new RegisterWriteRequest(
                    registerId, 
                    register.IntegerAddress, 
                    register.FloatAddress, 
                    (uint) register.WriteAddress, 
                    request.Value);
                
                return await this.ioDriver.Write(fakeRequest);
            }

            return await this.ioDriver.Write(request);
        }
    }
}
