namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using DispatcherDesktop.Device;
    using DispatcherDesktop.Device.Configuration;
    using DispatcherDesktop.Device.Data;
    using DispatcherDesktop.Device.Survey;
    using DispatcherDesktop.Models;

    using Prism.Commands;
    using Prism.Mvvm;

    using Unity.Attributes;

    public class DeviceDetailViewModel : BindableBase
    {
        private readonly IStorage storage;

        private readonly IDevicesConfigurationProvider devicesConfiguration;

        private DeviceDescription device;

        private ObservableCollection<Register> registers;

        public DeviceDetailViewModel(IDevicesConfigurationProvider devicesConfiguration, IStorage storage)
        {
            this.devicesConfiguration = devicesConfiguration;
            this.storage = storage;

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

        public ObservableCollection<Register> Registers
        {
            get => this.registers;
            set => this.SetProperty(ref this.registers, value);
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
                               Data = this.storage.Get(new RegisterId(this.device, r)).LastOrDefault(),
                           }); 

            this.Registers = new ObservableCollection<Register>(registerModels);
        }
    }
}
