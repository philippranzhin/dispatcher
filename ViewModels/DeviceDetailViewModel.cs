namespace DIspatcherDesktop.ViewModels
{
    using DIspatcherDesktop.Model;

    using Prism.Mvvm;
    public class DeviceDetailViewModel : BindableBase
    {
        private DeviceDescription device;

        public DeviceDetailViewModel()
        {}

        public DeviceDescription Device
        {
            get => this.device;
            set => this.SetProperty(ref this.device, value);
        }
    }
}
