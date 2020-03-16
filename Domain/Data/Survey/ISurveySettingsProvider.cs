namespace DispatcherDesktop.Domain.Data.Survey
{
    public interface  ISurveySettingsProvider
    {
        string ConnectionString { get; set; }

        int SurveyPeriodSeconds { get; set; }

		bool SurveyEnabled { get; set; }
    }
}
