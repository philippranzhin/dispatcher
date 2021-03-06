﻿namespace DispatcherDesktop.Domain.Data.Survey
{
    using Properties;

    public class SurveySettingsProvider : ISurveySettingsProvider
    {
        public string ConnectionString
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

        public bool SurveyEnabled
        {
            get => Settings.Default.SurveyEnabled;
            set
            {
                Settings.Default.SurveyEnabled = value;
                Settings.Default.Save();
            }
        }
    }
}
