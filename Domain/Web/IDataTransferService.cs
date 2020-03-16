namespace DispatcherDesktop.Domain.Web
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Model;
    using Models;

    public interface IDataTransferService : IDisposable
    {
        Task Connect(ConnectInfo connectInfo);

        Task Disconnect();
        
        bool Connected { get; }

        void SendConfig(ICollection<DeviceDescription> devices);
        
        void PushData(Device device);

        event Action<WriteRegister> WriteRequested;
        
        event Action ConfigRequested;
    }
}