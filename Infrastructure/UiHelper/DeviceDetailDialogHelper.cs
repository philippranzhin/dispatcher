namespace DispatcherDesktop.Infrastructure
{
    using System;

    public static class DeviceDetailDialogHelper
    {
        public static void RequestClose(object sender)
        {
            CloseRequested?.Invoke(sender, EventArgs.Empty);
        }

        public static event EventHandler CloseRequested;
    }
}
