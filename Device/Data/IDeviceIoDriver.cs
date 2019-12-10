namespace DispatcherDesktop.Device.Data
{
    using System.Threading.Tasks;

    using Models;

    public interface IDeviceIoDriver
    {
        Task Read(DeviceDescription description);

        Task<bool> Write(RegisterWriteData request);
    }
}
