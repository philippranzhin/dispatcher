namespace DispatcherDesktop.Domain.Data.Models
{
    using System;

    public class RegisterDataSlice
    {
        public RegisterDataSlice(RegisterId id, DateTime saveDate, double value)
        {
            this.Id = id;
            this.SaveDate = saveDate;
            this.Value = value;
        }

        public RegisterId Id { get; }

        public DateTime SaveDate { get; }

        public double Value { get ;}
    }
}
