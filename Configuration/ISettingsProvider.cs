namespace DispatcherDesktop.Configuration
{
    public interface ISettingsProvider
    {
        string ComName { get; set; }

        int SurveyPeriodSeconds { get; set; }
    }
}
