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
        private bool registerAdding;
        private bool registerValueWriting;
        private RegisterReference registerToWriteValue;

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
                this.UpdateRegisterData();
            }
        }

        public RegisterReference RegisterToWriteValue
        {
            get => this.registerToWriteValue;
            set => this.SetProperty(ref this.registerToWriteValue, value);
        }

        public ObservableCollection<Register> Registers
        {
            get => this.registers;
            set => this.SetProperty(ref this.registers, value);
        }

        public bool RegisterAdding
        {
            get => this.registerAdding;
            set => this.SetProperty(ref this.registerAdding, value);
        }

        public bool RegisterValueWriting
        {
            get => this.registerValueWriting;
            set => this.SetProperty(ref this.registerValueWriting, value);
        }

        public ICommand AddRegisterCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
                {
                    var newReg = new RegisterDescription()
                                     {
                                         Description = register.Description,
                                         Name = register.Name,
                                         IntegerAddress = register.IntegerAddress,
                                         FloatAddress = register.FloatAddress,
                                         Postfix = register.Postfix,
                                         WriteAddress = register.WriteAddress
                                     };
                    this.device.Registers.Add(newReg);
                    this.devicesConfiguration.Save(this.device);
                });

        public ICommand RemoveRegisterCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
                {
                    this.device.Registers.Remove(register);
                    this.devicesConfiguration.Save(this.device);
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
