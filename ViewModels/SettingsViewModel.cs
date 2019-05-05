namespace DispatcherDesktop.ViewModels
{
    using System.Windows.Input;

    using DispatcherDesktop.Configuration;
    using DispatcherDesktop.Device;

    using Prism.Commands;
    using Prism.Mvvm;
    public class SettingsViewModel : BindableBase
    {
        private bool surveyEnabled;

        private readonly ISurveyService surveyService;

        private readonly ISettingsProvider settingsProvider;

        private string portName;

        private int surveyPeriod;

        public SettingsViewModel(ISurveyService surveyService, ISettingsProvider settingsProvider)
        {
            this.surveyService = surveyService;
            this.settingsProvider = settingsProvider;

            this.surveyEnabled = surveyService.SurveyStarted;
            this.surveyPeriod = settingsProvider.SurveyPeriodSeconds;
            this.portName = settingsProvider.ConnectionString; 
        }

        public bool SurveyEnabled
        {
            get => this.surveyEnabled;
            set => this.SetProperty(ref this.surveyEnabled, value);
        }

        public string PortName
        {
            get => this.portName;
            set => this.SetProperty(ref this.portName, value);
        }

        public int SurveyPeriod
        {
            get => this.surveyPeriod;
            set => this.SetProperty(ref this.surveyPeriod, value);
        }

        public ICommand SaveConnectionSettingsCommand => new DelegateCommand(this.SaveConnectionSettings);

        private void SaveConnectionSettings()
        {
            this.settingsProvider.SurveyPeriodSeconds = this.SurveyPeriod;
            this.surveyService.SurveyStarted = this.SurveyEnabled;
            this.settingsProvider.ConnectionString = this.PortName;
        }
    }
}
