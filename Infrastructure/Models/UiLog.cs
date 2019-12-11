namespace DispatcherDesktop.Infrastructure.Models
{
    public class UiLog
    {
        public UiLog(int severity, string body)
        {
            this.Severity = severity;
            this.Body = body;
        }

        public int Severity { get; }

        public string Body { get; }
    }
}
