namespace DispatcherDesktop.Device
{
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    interface IDeviceDataReader
    {
        Task<bool> Read(DeviceDescription description);
    }
}
