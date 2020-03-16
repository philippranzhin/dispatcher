namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Domain.Configuration;
    using Domain.Data.Models;
    using Domain.Data.Storage;
    using Domain.Data.Survey;
    using Domain.Models;
    using Infrastructure.ViewContext;
    using Models;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DeviceDetailViewModel : BindableBase
    {
        private readonly IStorage storage;

        private readonly IDevicesConfigurationProvider devicesConfiguration;

        private readonly ISurveyService surveyService;

        private DeviceDescription device;

        private ObservableCollection<Register> registers;
        private bool registerEditing;
        private bool registerValueWriting;

        private SubViewDialogContext<RegisterReference> editContext;
        private SubViewDialogContext<RegisterReference> writeContext;

        public DeviceDetailViewModel(
            IDevicesConfigurationProvider devicesConfiguration, 
            IStorage storage, 
            ISurveyService surveyService)
        {
            this.devicesConfiguration = devicesConfiguration;
            this.storage = storage;
            this.surveyService = surveyService;

            this.devicesConfiguration.Saved += (s, e) => this.UpdateDeviceData();
        }
        
        public DeviceDescription Device
        {
            get => this.device;
            set
            {
                if (this.device == null)
                {
                    this.storage.Saved += this.UpdateRegisterData;
                }

                this.SetProperty(ref this.device, value);

                this.EditContext = new SubViewDialogContext<RegisterReference>();
                this.WriteContext = new SubViewDialogContext<RegisterReference>();

                this.UpdateDeviceData();
            }
        }

        public SubViewDialogContext<RegisterReference> EditContext
        {
            get => this.editContext;
            set => this.SetProperty(ref this.editContext, value);
        }

        public SubViewDialogContext<RegisterReference> WriteContext
        {
            get => this.writeContext;
            set => this.SetProperty(ref this.writeContext, value);
        }
         
        public ObservableCollection<Register> Registers
        {
            get => this.registers;
            set => this.SetProperty(ref this.registers, value);
        }

        public bool RegisterEditing
        {
            get => this.registerEditing;
            set => this.SetProperty(ref this.registerEditing, value);
        }

        public bool RegisterValueWriting
        {
            get => this.registerValueWriting;
            set => this.SetProperty(ref this.registerValueWriting, value);
        }

        public ICommand RemoveRegisterCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
                {
                    this.device.Registers.Remove(register);
                    this.devicesConfiguration.AddDevice(this.device);
                });

        public ICommand StartEditRegisterCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
            {
                this.RegisterEditing = true;
                this.EditContext.Start(new RegisterReference(this.Device, register ?? RegisterDescription.Empty));
            });
        
        public ICommand StartWriteCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
            {
                this.RegisterValueWriting = true;
                this.WriteContext.Start(new RegisterReference(this.Device, register));
            });

        
        private void UpdateRegisterData(object sender, RegisterDataSlice registerDataSlice)
        {
            if (this.device == null) return;

            if (registerDataSlice.Id.Device == this.device.Id)
            {
                var registerToUpdate = this.Registers.FirstOrDefault(r => r.Description.IntegerAddress == registerDataSlice.Id.Register);

                if (registerToUpdate != null)
                {
                    registerToUpdate.DataSlice = registerDataSlice;
                }
            }
        }
        
        private void UpdateDeviceData()
        {
            if (this.device == null || !this.devicesConfiguration.Devices.Any())
            {
                this.Registers = new ObservableCollection<Register>();
                return;
            }

            var updatedDevice = this.devicesConfiguration.Devices.First(d => d.Id == this.device.Id);
            
            var registerModels = updatedDevice.Registers.Select(
                (r) => new Register(r)
                {
                    DataSlice = this.storage.Get(new RegisterId(this.device.Id, r)).LastOrDefault(),
                });

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                this.Registers = new ObservableCollection<Register>(registerModels);
            });
        }
    }
}
