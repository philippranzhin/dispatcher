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

    public partial class DeviceDetail : UserControl
    {
        public DeviceDetail()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.DevicePropertyChanged;

            DeviceDetailDialogHelper.CloseRequested += (s, e) =>
            {
                this.Dispatcher?.Invoke(() =>
                {
                    this.Dialog.IsOpen = false;

                    if (!(this.DataContext is DeviceDetailViewModel viewModel))
                    {
                        return;
                    }

                    viewModel.RegisterAdding = false;
                    viewModel.RegisterValueWriting = false;
                });
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

        private void AddRegisterButton_OnClick(object sender, RoutedEventArgs e)
        {
            ((DeviceDetailViewModel)this.DataContext).RegisterAdding = true;
        }

        private void OnWriteRegisterValue(object sender, RoutedEventArgs e)
        {
            if (!((sender as Control)?.DataContext is Register register))
            {
                return;
            }

            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            }

            viewModel.RegisterValueWriting = true;
            viewModel.RegisterToWriteValue = new RegisterReference(viewModel.Device.Id, register.Description);
            DialogHost.OpenDialogCommand.Execute(null, null);
        }
    }
}