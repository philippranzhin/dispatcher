namespace DispatcherDesktop.Views
{
    using System.Windows.Controls;

    using DispatcherDesktop.ViewModels;

    using MaterialDesignThemes.Wpf;

    /// <summary>
    /// Interaction logic for DeviceWall.xaml
    /// </summary>
    public partial class DeviceWall : UserControl
    {
        public DeviceWall()
        {
            this.InitializeComponent();
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
        {
            (this.DataContext as DeviceWallViewModel)?.AddDeviceCommand?.Execute(eventargs.Parameter);
        }
    }
}
