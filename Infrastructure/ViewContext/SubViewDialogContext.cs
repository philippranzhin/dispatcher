namespace DispatcherDesktop.Infrastructure.ViewContext
{
    using System;

    public class SubViewDialogContext<TPayload>
    {
        public event EventHandler OnFinish;

        public event EventHandler<TPayload> OnStart;

        public event EventHandler OnCancel;

        public void Finish()
        {
            this.OnFinish?.Invoke(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            this.OnCancel?.Invoke(this, EventArgs.Empty);
        }

        public void Start(TPayload payload)
        {
            this.OnStart?.Invoke(this, payload);
        }
    }
}
