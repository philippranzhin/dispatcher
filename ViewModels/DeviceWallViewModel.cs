namespace DispatcherDesktop.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;

    using Device;
    using DispatcherDesktop.Device.Configuration;
    using Device.Survey;
    using Models;

    using Prism.Commands;
    using Prism.Mvvm;

    public class DeviceWallViewModel : BindableBase
    {
        private readonly ISurveyService surveyService;

        private ObservableCollection<DeviceDescription> devices;

        private readonly IDevicesConfigurationProvider devicesConfiguration;

        private DeviceDescription selectedDevice;

        private string addingDeviceName;

        private uint? addingDeviceId;

        private bool surveyStarted;

        public DeviceWallViewModel(IDevicesConfigurationProvider configurationProvider, ISurveyService surveyService, IDevicesConfigurationProvider devicesConfiguration)
        {
            this.devices = new ObservableCollection<DeviceDescription>(configurationProvider.Devices);
            this.surveyService = surveyService;
            this.devicesConfiguration = devicesConfiguration;
            this.surveyStarted = this.surveyService.SurveyStarted;

            this.surveyService.SurveyStartedChanged += (s, e) =>
                    {
                        this.SurveyStarted = this.surveyService.SurveyStarted;
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

        public string AddingDeviceName
        {
            get => this.addingDeviceName;
            set
            {
                this.SetProperty(ref this.addingDeviceName, value);
                this.RaisePropertyChanged(nameof(this.CanSave));
            }
        }
        public uint? AddingDeviceId
        {
            get => this.addingDeviceId;
            set
            {
                this.SetProperty(ref this.addingDeviceId, value);
                this.RaisePropertyChanged(nameof(this.CanSave));
            } 
        }

        public bool CanSave => this.AddingDeviceId != null && !string.IsNullOrWhiteSpace(this.AddingDeviceName);
         
        public ICommand AddDeviceCommand => new DelegateCommand<bool?>(
            (obj) =>
                {
                    if (obj != null && (obj.Value && this.CanSave))
                    {
                        var deviceId = this.addingDeviceId;
                        if (deviceId != null)
                            this.devices.Add(
                                new DeviceDescription(
                                    (uint)deviceId,
                                    this.addingDeviceName,
                                    new List<RegisterDescription>()));

                        this.devicesConfiguration.Save(this.devices);
                        this.AddingDeviceId = null;
                        this.AddingDeviceName = null;
                    }
                });

        public ICommand RemoveDeviceCommand => new DelegateCommand<DeviceDescription>(
            (deviceDescription) =>
                {
                    if (deviceDescription != null)
                    {
                        this.devices.Remove(deviceDescription);

                        this.devicesConfiguration.Save(this.devices);
                        this.SelectedDevice = this.devices.LastOrDefault();
                    }
                });
    }
}
