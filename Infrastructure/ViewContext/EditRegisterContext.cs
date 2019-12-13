namespace DispatcherDesktop.Infrastructure.ViewContext
{
    using System;
    using DispatcherDesktop.Models;

    public class EditRegisterContext
    {
        public EditRegisterContext(DeviceDescription selectedDevice)
        {
            this.SelectedDevice = selectedDevice;
        }

        public event EventHandler OnFinish;

        public event EventHandler<RegisterDescription> OnStart;

        public event EventHandler OnCancel;

        public DeviceDescription SelectedDevice { get; }

        public void Finish()
        {
            this.OnFinish?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            this.OnCancel?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Request register edit or create.
        /// </summary>
        /// <param name="register">Register to edit. If null, should be created new.</param>
        public void Start(RegisterDescription register = null)
        {
            this.OnStart?.Invoke(this, register);
        }
    }
}
