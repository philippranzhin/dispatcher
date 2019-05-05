namespace DispatcherDesktop.Device.Survey
{
    using System;

    public interface ISurveyService
    {
        bool SurveyStarted { get; set; }

        event EventHandler<bool> ServeyStartedChanged;
    }
}
 