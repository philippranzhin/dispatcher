namespace DispatcherDesktop.Models
{
    public class DeviceDescription
    {
        public DeviceDescription(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }

        public int Id { get ; }

        public string Name { get; }
    }
}
