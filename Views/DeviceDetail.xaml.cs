using DispatcherDesktop.Device.Data;

namespace DispatcherDesktop.Views
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using Infrastructure;
    using Infrastructure.ViewContext;
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

                    viewModel.RegisterEditing = false;
                    viewModel.RegisterValueWriting = false;
                });
            };
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var device = (DeviceDescription)context.Value;

            if (this.DataContext is DeviceDetailViewModel viewModel)
            {
                viewModel.Device = device;

                viewModel.EditContext.OnCancel += this.HandleEditingFinish;
                viewModel.EditContext.OnFinish += this.HandleEditingFinish;
            }
        }

        private void HandleEditingFinish(object sender, EventArgs e)
        {
            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            }

            this.Dialog.IsOpen = false;

            viewModel.RegisterEditing = false;
        }

        private void OnDialogClosing(object sender, DialogClosingEventArgs eventargs)
        {
            eventargs.Handled = true;

            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            } 

            viewModel.RegisterValueWriting = false;
        }

        private void OnStartCreateRegister(object sender, RoutedEventArgs e)
        {
            if (!(this.DataContext is DeviceDetailViewModel viewModel))
            {
                return;
            }

            viewModel.RegisterEditing = true;

            viewModel.EditContext?.Start();
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

        private void OnStartEditRegister(object sender, RoutedEventArgs e)
        {
            this.Dialog.IsOpen = true;
        }
    }
}