namespace DispatcherDesktop.ViewModels
{
    using System.Windows.Input;
    using Device.Survey;

    using Prism.Commands;
    using Prism.Mvvm;
    public class SettingsViewModel : BindableBase
    {
        private bool surveyEnabled;

        private readonly ISurveyService surveyService;

        private readonly ISurveySettingsProvider surveySettingsProvider;

        private string portName;

        private int surveyPeriod;

        public SettingsViewModel(ISurveyService surveyService, ISurveySettingsProvider surveySettingsProvider)
        {
            this.surveyService = surveyService;
            this.surveySettingsProvider = surveySettingsProvider;

			surveyService.SurveyStarted = surveySettingsProvider.SurveyEnabled;
			this.surveyEnabled = surveySettingsProvider.SurveyEnabled;

            this.surveyPeriod = surveySettingsProvider.SurveyPeriodSeconds;
            this.portName = surveySettingsProvider.ConnectionString; 
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
			this.surveyService.SurveyStarted = this.SurveyEnabled;

			this.surveySettingsProvider.SurveyEnabled = this.SurveyEnabled;
			this.surveySettingsProvider.SurveyPeriodSeconds = this.SurveyPeriod;
            this.surveySettingsProvider.ConnectionString = this.PortName;
        }
    }
}
