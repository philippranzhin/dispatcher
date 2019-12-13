namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;
    using Device;
    using DispatcherDesktop.Device.Configuration;
    using Device.Data;
    using Device.Survey;
    using Infrastructure.ViewContext;
    using Models;

    using Prism.Commands;
    using Prism.Mvvm;

    using Unity.Attributes;

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

            this.devicesConfiguration.Saved += ((s, e) => this.UpdateRegisterData());
        }

        public DeviceDescription Device
        {
            get => this.device;
            set
            {
                if (this.device == null)
                {
                    this.storage.Saved += (s, e) =>
                        {
                            if (this.device != null)
                            {
                                this.UpdateRegisterData();
                            }
                        };
                }

                this.SetProperty(ref this.device, value);

                this.EditContext = new SubViewDialogContext<RegisterReference>();
                this.WriteContext = new SubViewDialogContext<RegisterReference>();

                this.UpdateRegisterData();
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
                    this.devicesConfiguration.Save(this.device);
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

        private void UpdateRegisterData()
        {
            if (this.device == null)
            {
                this.Registers = new ObservableCollection<Register>();
                return;
            }

            var registerModels = this.device.Registers.Select(
                (r) => new Register(r)
                           {
                               Data = this.storage.Get(new RegisterId(this.device.Id, r)).LastOrDefault(),
                           });

            Dispatcher.CurrentDispatcher.Invoke(() =>
            {
                this.Registers = new ObservableCollection<Register>(registerModels);
            });
        }
    }
}
