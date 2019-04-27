namespace DispatcherDesktop.ViewModels
{
    using DispatcherDesktop.Device;
    using DispatcherDesktop.Models;

    using Prism.Mvvm;

    public class DeviceDetailViewModel : BindableBase
    {
        private readonly IDeviceDataProvider dataProvider;

        private DeviceDescription device;

        private DeviceData deviceData;

        public DeviceDetailViewModel(IDeviceDataProvider dataProvider)
        {
            this.dataProvider = dataProvider;
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
                            if (this.device != null && e.Address == this.device.Id)
                            {
                                this.DeviceData = e;
                            }
                        };
                }

                this.DeviceData = 
                    this.dataProvider.RecentData.TryGetValue(value.Id, out var recentDeviceData) 
                        ? recentDeviceData 
                        : null;

                this.SetProperty(ref this.device, value);
            }
        }

        public DeviceData DeviceData
        {
            get => this.deviceData;
            set => this.SetProperty(ref this.deviceData, value);
        }
    }
}
