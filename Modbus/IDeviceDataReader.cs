namespace DispatcherDesktop.Modbus
{
    using System.Threading.Tasks;

    using DispatcherDesktop.Models;

    interface IDeviceDataReader
    {
        Task<DeviceData> Read(DeviceDescription description);
    }
}
