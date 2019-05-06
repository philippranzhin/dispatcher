namespace DispatcherDesktop.Views
{
    using System.Windows;
    using System.Windows.Controls;

    using DispatcherDesktop.Models;
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

        private void ListBoxItem_OnSelected(object sender, RoutedEventArgs e)
        {
            if ((sender as Control)?.DataContext is DeviceDescription description)
            {
                (this.DataContext as DeviceWallViewModel)?.RemoveDeviceCommand.Execute(description);
            }
        }
    }
}
