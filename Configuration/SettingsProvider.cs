namespace DispatcherDesktop.Configuration
{
    using DispatcherDesktop.Properties;

    public class SettingsProvider : ISettingsProvider
    {
        public string ComName
        {
            get => Settings.Default.PortName;
            set
            {
                Settings.Default.PortName = value;
                Settings.Default.Save();
            }
        }

        public int SurveyPeriodSeconds
        {
            get => Settings.Default.SurveyPeriod;
            set
            {
                Settings.Default.SurveyPeriod = value;
                Settings.Default.Save();
            }
        }
    }
}
