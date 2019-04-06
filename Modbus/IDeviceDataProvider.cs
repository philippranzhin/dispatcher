namespace DispatcherDesktop.Modbus
{
    using System;
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public interface IDeviceDataProvider
    {
        Dictionary<int, DeviceData> RecentData { get; }

        bool SurveyStarted { get; set; }

        event EventHandler<DeviceData> DataReceived;

        event EventHandler<bool> ServeyStartedChanged;
    }
}
