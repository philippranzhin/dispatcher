using DispatcherDesktop.Device.Data;

namespace DispatcherDesktop.Views
{
    using System.Windows;
    using System.Windows.Controls;
    using Infrastructure;
    using Models;
    using ViewModels;

    using MaterialDesignThemes.Wpf;

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

            DeviceDetailDialogHelper.CloseRequested += (s, e) =>
            {
                Dispatcher.Invoke(() => { this.Dialog.IsOpen = false; });
            };
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var device = (DeviceDescription)context.Value;
            ((DeviceDetailViewModel)this.DataContext).Device = device;
        }

        private void OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
        {
            eventargs.Handled = true;

            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            } 

            viewModel.RegisterAdding = false;
            viewModel.RegisterValueWriting = false;

            if ((eventargs.Parameter as RegisterDescription) == null)
            {
                return;
            }

            viewModel.AddRegisterCommand?.Execute(eventargs.Parameter);
        }
         
        private void OnRemoveRegister(object sender, RoutedEventArgs e)
        {
            if ((sender as Control)?.DataContext is RegisterDescription description)
            {
                (this.DataContext as DeviceDetailViewModel)?.RemoveRegisterCommand.Execute(description);
            }
        }

        private void AddRegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ((DeviceDetailViewModel)this.DataContext).RegisterAdding = true;
        }

        private void OnWriteRegisterValue(object sender, RoutedEventArgs e)
        {
            if (!((sender as Control)?.DataContext is RegisterDescription description))
            {
                return;
            }

            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            }

            viewModel.RegisterValueWriting = true;
            viewModel.RegisterToWriteValue = new RegisterReference(viewModel.Device.Id, description);
            DialogHost.OpenDialogCommand.Execute(null, null);
        }

        private void OnRegisterCardPopupOpen(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            }

            viewModel.Pause();
        }
    }
}