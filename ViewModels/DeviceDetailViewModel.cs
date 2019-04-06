namespace DispatcherDesktop.ViewModels
{
    using System.Linq;

    using DispatcherDesktop.Modbus;
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

            this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == "DeviceData")
                    {
                    }
                };
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

                if (this.dataProvider.RecentData.TryGetValue(value.Id, out var recentDeviceData))
                {
                    this.DeviceData = recentDeviceData;
                }
                else
                {
                    this.DeviceData = null;
                }

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
