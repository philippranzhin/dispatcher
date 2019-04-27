namespace DispatcherDesktop.ViewModels
{
    using System.Collections.ObjectModel;

    using DispatcherDesktop.Device;
    using DispatcherDesktop.Models;

    using Prism.Mvvm;

    public class DeviceWallViewModel : BindableBase
    {
        private readonly IDeviceDataProvider deviceDataProvider;

        private ObservableCollection<DeviceDescription> devices;

        private DeviceDescription selectedDevice;

        private bool surveyStarted;

        public DeviceWallViewModel(IDevicesConfigurationProvider configurationProvider, IDeviceDataProvider deviceDataProvider)
        {
            this.devices = new ObservableCollection<DeviceDescription>(configurationProvider.Devices);
            this.deviceDataProvider = deviceDataProvider;
            this.surveyStarted = this.deviceDataProvider.SurveyStarted;

            this.deviceDataProvider.ServeyStartedChanged += (s, e) =>
                    {
                        this.SurveyStarted = this.deviceDataProvider.SurveyStarted;
                    };
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

        public bool SurveyStarted
        {
            get => this.surveyStarted;
            set => this.SetProperty(ref this.surveyStarted, value);
        }
    }
}
