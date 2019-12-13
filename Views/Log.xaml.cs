using System.Windows;
using System.Windows.Controls;

namespace DispatcherDesktop.Views
{
    using ViewModels;

    /// <summary>
    /// Interaction logic for Log.xaml
    /// </summary>
    public partial class Log : UserControl
    {
        public Log()
        {
            this.InitializeComponent();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (this.DataContext is LogViewModel vm)
            {
                vm.Logs.CollectionChanged += (s, ea) =>
                {
                    this.Dispatcher.Invoke(() => this.ConsoleScroll.ScrollToEnd());
                };
            }
        }
    }
}
