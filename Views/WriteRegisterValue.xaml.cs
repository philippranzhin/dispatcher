using System.Windows.Controls;
using DispatcherDesktop.Models;
using DispatcherDesktop.ViewModels;
using Prism.Common;
using Prism.Regions;

namespace DispatcherDesktop.Views
{
    using Infrastructure.ViewContext;

    public partial class WriteRegisterValue : UserControl
    {
        public WriteRegisterValue()
        {
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.RegisterPropertyChanged;
        }

        private void RegisterPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var contextValue = (SubViewDialogContext<RegisterReference>)context.Value;

            ((WriteRegisterValueViewModel)this.DataContext).Context = contextValue;
        }
    }
}
