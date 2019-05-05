namespace DispatcherDesktop.Device
{
    using System;
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public interface ISurveyService
    {
        bool SurveyStarted { get; set; }

        event EventHandler<uint> DataReceived;

        event EventHandler<bool> ServeyStartedChanged;
    }
}
