namespace DispatcherDesktop.Domain.Configuration.Models
{
    using System.Collections.Generic;
    using Domain.Models;
    using Models;

    public class DeviceConfig
    {
        public ICollection<DeviceDescription> Devices { get; set; } = new List<DeviceDescription>();
        
        public ICollection<DeviceMapper> Mappers { get; set; } = new List<DeviceMapper>( );
    }
}