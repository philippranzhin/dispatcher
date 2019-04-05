namespace DIspatcherDesktop.ViewModels
{
    using System.Collections.ObjectModel;

    using DIspatcherDesktop.Model;

    using Prism.Mvvm;

    public class DeviceWallViewModel : BindableBase
    {
        private ObservableCollection<DeviceDescription> devices;

        public DeviceWallViewModel()
        {
            this.devices = new ObservableCollection<DeviceDescription>()
                                   {
                                       new DeviceDescription(1, "first"),
                                       new DeviceDescription(2, "second")
                                   };
        }

        public ObservableCollection<DeviceDescription> Devices
        {
            get => this.devices;
            set => this.SetProperty(ref this.devices, value);
        }
    }
}
