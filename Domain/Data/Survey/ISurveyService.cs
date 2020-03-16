namespace DispatcherDesktop.Domain.Data.Survey
{
    using System;
    using Models;

    public interface ISurveyService
    {
        bool SurveyStarted { get; set; }

        void ScheduleWriteOperation(RegisterWriteRequest request, Action<bool> onFinish);

        void ForceSurvey();

        event EventHandler<bool> SurveyStartedChanged;

        event Action<DeviceDataSlice> DeviceDataReceived;
    }
}
 