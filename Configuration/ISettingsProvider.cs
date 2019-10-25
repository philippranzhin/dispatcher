namespace DispatcherDesktop.Configuration
{
    public interface ISettingsProvider
    {
        string ConnectionString { get; set; }

        int SurveyPeriodSeconds { get; set; }

		bool SurveyEnabled { get; set; }
    }
}
