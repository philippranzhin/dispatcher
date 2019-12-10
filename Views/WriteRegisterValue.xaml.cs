using System.Windows.Controls;
using DispatcherDesktop.Device.Data;
using DispatcherDesktop.Models;
using DispatcherDesktop.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace DispatcherDesktop.Views
{
    using System;
    using System.Windows;
    using Infrastructure;
    using MaterialDesignThemes.Wpf;

    public partial class WriteRegisterValue : UserControl
    {
        public WriteRegisterValue()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegisterPropertyChanged;
            ((WriteRegisterValueViewModel) this.DataContext).OnOperationFinish += this.OnFinish;
        }

        private void RegisterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var register = (RegisterReference)context.Value;

            ((WriteRegisterValueViewModel)this.DataContext).Register = register;
        }

        private void OnFinish(object sender, EventArgs e)
        {
            DeviceDetailDialogHelper.RequestClose(sender);
        }
    }
}
