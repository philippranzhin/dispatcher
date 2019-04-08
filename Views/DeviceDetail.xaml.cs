namespace DispatcherDesktop.Views
{
    using System.Windows.Controls;

    using DispatcherDesktop.Models;
    using DispatcherDesktop.ViewModels;

    using Prism.Common;
    using Prism.Regions;

    /// <summary>
    /// Interaction logic for DeviceDetail.xaml
    /// </summary>
    public partial class DeviceDetail : UserControl
    {
        public DeviceDetail()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.DevicePropertyChanged;
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var device = (DeviceDescription)context.Value;
            (this.DataContext as DeviceDetailViewModel).Device = device;
        }
    }
}
