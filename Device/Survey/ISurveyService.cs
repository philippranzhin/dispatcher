namespace DispatcherDesktop.Device.Survey
{
    using System;
    using Models;

    public interface ISurveyService
    {
        bool SurveyStarted { get; set; }

        void ScheduleWriteOperation(RegisterWriteData request, Action<bool> onFinish);

        event EventHandler<bool> SurveyStartedChanged;
    }
}
 