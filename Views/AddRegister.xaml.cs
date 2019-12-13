using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DispatcherDesktop.Views
{
    using Infrastructure.ViewContext;
    using Models;
    using ViewModels;

    using Prism.Common;
    using Prism.Regions;

    /// <summary>
    /// Interaction logic for AddRegister.xaml
    /// </summary>
    public partial class AddRegister : UserControl
    {
        public AddRegister()
        { 
            this.InitializeComponent();
            RegionContext.GetObservableContext(this).PropertyChanged += this.DevicePropertyChanged;
        }

        private void DevicePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var context = (ObservableObject<object>)sender;
            var contextValue = (EditRegisterContext)context.Value;
            ((AddRegisterViewModel)this.DataContext).Context = contextValue;
        }

    }
}
