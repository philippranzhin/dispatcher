namespace DispatcherDesktop.ViewModels
{
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
                            if (this.device != null && e.Id.Device == this.device.Id)
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

        public ICommand AddRegisterCommand => new DelegateCommand<bool?>(
            (obj) =>
                {
                    var r = new RegisterDescription()
                                {

                                    Name = $"регистр #{this.Registers.Count}",
                                    Description = "описание",
                                    Postfix = "°",
                                    IntegerAddress = (uint)(this.Registers.Count + 1)
                                };

                    this.device.Registers.Add(r);
                    this.devicesConfiguration.Save(this.device);
                    this.UpdateRegisterData();
                });

        public ICommand RemoveRegisterCommand => new DelegateCommand<RegisterDescription>(
            (register) =>
                {
                    this.device.Registers.Remove(register);
                    this.devicesConfiguration.Save(this.device);
                    this.UpdateRegisterData();
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
