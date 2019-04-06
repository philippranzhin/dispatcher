namespace DispatcherDesktop.ViewModels
{
    using DispatcherDesktop.Modbus;

    using Prism.Mvvm;

    public class MainWindowViewModel : BindableBase
    {
        private readonly IDeviceDataProvider deviceDataProvider;

        private bool surveyEnabled;

        public MainWindowViewModel(IDeviceDataProvider deviceDataProvider)
        {
            this.deviceDataProvider = deviceDataProvider;

            this.surveyEnabled = deviceDataProvider.SurveyStarted;
        }

        public bool SurveyEnabled
        {
            get => this.surveyEnabled;
            set
            {
                this.deviceDataProvider.SurveyStarted = value;
                this.SetProperty(ref this.surveyEnabled, value);
            }
        }
    }
}
