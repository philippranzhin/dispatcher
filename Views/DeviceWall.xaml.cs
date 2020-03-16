namespace DispatcherDesktop.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Domain.Models;
    using ViewModels;

    using MaterialDesignThemes.Wpf;

    public partial class DeviceWall : UserControl
    {
        public DeviceWall()
        {
            this.InitializeComponent();
        }

        private void DialogHost_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            e.Handled = true;

            (this.DataContext as DeviceWallViewModel)?.AddDeviceCommand?.Execute(e.Parameter);
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
