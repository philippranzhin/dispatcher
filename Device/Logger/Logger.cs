namespace DispatcherDesktop.Device.Logger
{
    using System;
    using System.Windows.Threading;

    public class Logger : ILogger
    {
        public event EventHandler<string> InfoLogged;
        public event EventHandler<string> ErrorLogged;

        public void LogInfo(string log)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => { this.InfoLogged?.Invoke(this, log); });

        }

        public void LogError(string log)
        {
            Dispatcher.CurrentDispatcher.Invoke(() => { this.ErrorLogged?.Invoke(this, log); });
        }
    }
}
