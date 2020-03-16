namespace DispatcherDesktop.Domain.Data.Device
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Domain.Models;
    using Models;

    public interface IDeviceIoDriver
    {
        Task<List<RegisterDataSlice>> Read(DeviceDescription description);

        Task<bool> Write(RegisterWriteRequest request);
    }
}
