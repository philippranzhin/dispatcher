namespace DispatcherDesktop.Device.Survey
{
    using System;
    using Models;

    public interface ISurveyService
    {
        bool SurveyStarted { get; set; }

        void ScheduleWriteOperation(RegisterWriteData request, Action onSuccess);

        event EventHandler<bool> SurveyStartedChanged;

        void PauseOn(uint milliseconds);
    }
}
 