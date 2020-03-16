namespace DispatcherDesktop.Domain
{
    using System;
    using System.Threading.Tasks;
    using Web;

    public interface IDataManager : IDisposable
    {
        bool Started { get; }
        
        Task Start(ConnectInfo connectInfo);

        Task Stop();
    }
}