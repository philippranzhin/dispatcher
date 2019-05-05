namespace DispatcherDesktop.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    using DispatcherDesktop.Device;
    using DispatcherDesktop.Models;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DeviceDetailViewModel : BindableBase
    {
        private readonly ISurveyService dataProvider;

        private DeviceDescription device;

        private ObservableCollection<RegisterDescription> registers;

        private readonly IDevicesConfigurationProvider devicesConfiguration;

        public DeviceDetailViewModel(ISurveyService dataProvider, IDevicesConfigurationProvider devicesConfiguration)
        {
            this.dataProvider = dataProvider;
            this.devicesConfiguration = devicesConfiguration;
        }

        public DeviceDescription Device
        {
            get => this.device;
            set
            {
                if (this.device == null)
                {
                    this.dataProvider.DataReceived += (s, e) =>
                        {
                            if (this.device != null && e == this.device.Id)
                            {
                                this.RaisePropertyChanged(nameof(this.Device));
                                this.Registers = new ObservableCollection<RegisterDescription>(this.Device.Registers);
                            }
                        };
                }

                this.SetProperty(ref this.device, value);
                this.Registers = new ObservableCollection<RegisterDescription>(this.device.Registers);
            }
        }

        public ObservableCollection<RegisterDescription> Registers
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
                                    IntegerAddress = (uint)this.Registers.Count
                                };
                    r.Data.Add(this.Registers.Count);

                    this.device.Registers.Add(r);
                    this.registers.Add(r);

                    this.Registers = new ObservableCollection<RegisterDescription>(this.registers);
                    this.devicesConfiguration.Save();
                });
    }
}
