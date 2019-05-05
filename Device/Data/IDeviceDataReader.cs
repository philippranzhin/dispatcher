namespace DispatcherDesktop.Device.Data
{
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    interface IDeviceDataReader
    {
        Task Read(DeviceDescription description);
    }
}
