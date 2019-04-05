namespace DispatcherDesktop.ViewModels
{
    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private string title = "Prism Unity Application";
        public string Title
        {
            get => this.title;
            set => this.SetProperty(ref this.title, value);
        }

        public MainWindowViewModel()
        {
            this.Title = "xuy";
        }
    }
}
