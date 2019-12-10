namespace DispatcherDesktop.Models
{
    using System;

    using Device.Data;

    public class RegisterData
    {
        public RegisterData(RegisterId id, DateTime saveDate, double value)
        {
            this.Id = id;
            this.SaveDate = saveDate;
            this.Value = value;
        }

        public RegisterId Id { get; }

        public DateTime SaveDate { get ;}

        public double Value { get ;}
    }
}
