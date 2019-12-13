using System.Windows.Controls;

namespace DispatcherDesktop.Views
{
    using Infrastructure.ViewContext;
    using Models;
    using ViewModels;

    using Prism.Common;
    using Prism.Regions;

    /// <summary>
    /// Interaction logic for EditRegister.xaml
    /// </summary>
    public partial class EditRegister : UserControl
    {
        public EditRegister()
        { 
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.DevicePropertyChanged;
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var contextValue = (SubViewDialogContext<RegisterReference>)context.Value;
            ((EditRegisterViewModel)this.DataContext).Context = contextValue;
        }

    }
}
