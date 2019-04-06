namespace DispatcherDesktop.Models
{
    public class DeviceData
    {
        public DeviceData(string name, int address)
        {
            this.Name = name;
            this.Address = address;
            this.Seazon = Seazon.Winter;
            this.DirectTemperature = 0;
            this.GivingHeatingTemperature = 0;
            this.HeatingClosingLimit = 0;
            this.HeatingOpeningLimit = 0;
            this.MixingTemperature = 0;
            this.ReturnTemperature = 0;
            this.OutdoorTemperature = 0;
        }

        public string Name { get; }

        public int Address { get; }

        public Seazon Seazon { get; set; }

        public float OutdoorTemperature { get; set; }

        public float DirectTemperature { get; set; }

        public float ReturnTemperature { get; set; }

        public float MixingTemperature { get; set; }

        public float GivingHeatingTemperature { get; set; }

        public int HeatingOpeningLimit { get; set; }

        public int HeatingClosingLimit { get; set; }
    }
}
