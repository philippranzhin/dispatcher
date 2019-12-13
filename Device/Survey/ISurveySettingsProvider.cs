namespace DispatcherDesktop.Device.Survey
{
    public interface  ISurveySettingsProvider
    {
        string ConnectionString { get; set; }

        int SurveyPeriodSeconds { get; set; }

		bool SurveyEnabled { get; set; }
    }
}
