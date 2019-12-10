namespace DispatcherDesktop.Device.Logger
{
    using System;

    public class UiLogger : IUiLogger
    {
        public event EventHandler<string> InfoLogged;
        public event EventHandler<string> ErrorLogged;

        public void LogInfo(string log)
        {
            this.InfoLogged?.Invoke(this, log);
        }

        public void LogError(string log)
        {
            this.ErrorLogged?.Invoke(this, log);
        }
    }
}
