namespace DispatcherDesktop.ViewModels
{
    using System.Collections.ObjectModel;

    using DispatcherDesktop.Modbus;
    using DispatcherDesktop.Models;

    using Prism.Mvvm;

    public class DeviceWallViewModel : BindableBase
    {
        private readonly IDeviceDataProvider deviceDataProvider;

        private ObservableCollection<DeviceDescription> devices;

        private DeviceDescription selectedDevice;

        public DeviceWallViewModel(IDevicesConfigurationProvider configurationProvider, IDeviceDataProvider deviceDataProvider)
        {
            this.devices = new ObservableCollection<DeviceDescription>(configurationProvider.Devices);
            this.deviceDataProvider = deviceDataProvider;
        }

        public ObservableCollection<DeviceDescription> Devices
        {
            get => this.devices;
            set => this.SetProperty(ref this.devices, value);
        }

        public DeviceDescription SelectedDevice
        {
            get => this.selectedDevice;
            set => this.SetProperty(ref this.selectedDevice, value);
        }
    }
}
