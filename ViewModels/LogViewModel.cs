namespace DispatcherDesktop.ViewModels
{
    using Device.Logger;
    using Prism.Mvvm;

    public class LogViewModel : BindableBase
    {
        private string log;

        public LogViewModel(IUiLogger logger)
        {
            this.log = string.Empty;

            logger.InfoLogged += (sender, args) => { this.Log += $"\nInfo: {args}"; };
            logger.ErrorLogged += (sender, args) => { this.Log += $"\nError: {args}"; };
        }

        public string Log
        {
            get => this.log;
            set => this.SetProperty(ref this.log, value);
        }
    }
}
