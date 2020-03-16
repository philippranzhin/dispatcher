namespace DispatcherDesktop.Domain
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;
    using Configuration;
    using Data.Models;
    using Data.Storage;
    using Data.Survey;
    using Web;
    using Web.Model;

    public class DataManager : IDataManager
    {
        private readonly IDataTransferService dataTransferService;
        private readonly ISurveyService surveyService;
        private readonly IDevicesConfigurationProvider configurationProvider;
        private readonly IStorage storage;

        public DataManager(
            IDataTransferService dataTransferService, 
            ISurveyService surveyService, 
            IStorage storage, 
            IDevicesConfigurationProvider configurationProvider)
        {
            this.dataTransferService = dataTransferService;
            this.surveyService = surveyService;
            this.storage = storage;
            this.configurationProvider = configurationProvider;

            this.surveyService.DeviceDataReceived += this.HandleDataReceive;

            this.dataTransferService.WriteRequested += this.HandleWriteRequest;
            this.dataTransferService.ConfigRequested += this.HandleConfigRequest;
        }

        private void HandleConfigRequest()
        {
            this.dataTransferService.SendConfig(this.configurationProvider.Devices);
            Task.Run(async () =>
            {
                await Task.Delay(1000);
                this.surveyService.ForceSurvey(); 
            });
        }

        private void HandleWriteRequest(WriteRegister writeRegisterParams)
        {
            var description =
                this.configurationProvider.Devices
                    .FirstOrDefault(d => d.Id.ToString() == writeRegisterParams.Device)
                    ?.Registers.FirstOrDefault(r => r.IntegerAddress.ToString() == writeRegisterParams.Register);
            
            if (description?.WriteAddress == null)
            {
                return;
            }
            
            var registerId = new RegisterId(Convert.ToUInt32(writeRegisterParams.Device), description);
            var value = Convert.ToDouble(writeRegisterParams.Value);
            
            this
                .surveyService
                .ScheduleWriteOperation(
                    new RegisterWriteRequest(registerId, description.IntegerAddress, description.FloatAddress, (uint)description.WriteAddress, value), 
                    writeRegisterParams.onFinish);
        }

        private void HandleDataReceive(DeviceDataSlice data)
        {
            var registers = data
                .RegisterDataSlices
                .Select(rd =>
                {
                    this.storage.Save(rd);
                    
                    return new Register(rd.Id.Register.ToString(), rd.Value.ToString("F1", CultureInfo.InvariantCulture));
                })
                .ToList();
            
            if (!this.dataTransferService.Connected) return;
            
            this.dataTransferService.PushData(new Device(data.DeviceId.ToString(), registers));
        }

        public bool Started => this.dataTransferService.Connected;

        public async Task Start(ConnectInfo connectInfo)  
        {
            await this.dataTransferService.Connect(connectInfo);
        }

        public async Task Stop()
        {
            await this.dataTransferService.Disconnect();
        }

        public void Dispose()
        {
            this.dataTransferService?.Dispose();
        }
    }
}