namespace DispatcherDesktop.Device.Logger
{
    using System;

    public interface IUiLogger
    {
        event EventHandler<string> InfoLogged;
        event EventHandler<string> ErrorLogged;

        void LogInfo(string log);

        void LogError(string log);
    }
}
