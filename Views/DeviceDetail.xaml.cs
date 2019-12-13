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

                viewModel.WriteContext.OnCancel += this.HandleWritingFinish;
                viewModel.WriteContext.OnFinish += this.HandleWritingFinish;
            }
        }

        private void HandleWritingFinish(object sender, EventArgs e)
        {
            this.Dispatcher?.Invoke(() => {
                if (!(this.DataContext is DeviceDetailViewModel viewModel))
                {
                    return;
                }

                this.Dialog.IsOpen = false; 
                viewModel.RegisterValueWriting = false;
            });
        }

        private void HandleEditingFinish(object sender, EventArgs e)
        {
            this.Dispatcher?.Invoke(() => {
                if (!(this.DataContext is DeviceDetailViewModel viewModel))
                {
                    return;
                }

                this.Dialog.IsOpen = false;
                viewModel.RegisterEditing = false;
            });
        }

        private void OpenDialog(object sender, RoutedEventArgs e)
        {
            this.Dialog.IsOpen = true;
        }
    }
}