namespace DispatcherDesktop.Domain.Web.Model
{
    using System;

    public class WriteRegister
    {
        public WriteRegister(string device, string register, string value, Action<bool> onFinish)
        {
            this.Device = device;
            this.Register = register;
            this.Value = value;
            this.onFinish = onFinish;
        }

        public string Device { get; }
        
        public string Register { get; }
        
        public string Value { get; }

        public Action<bool> onFinish { get; }
    }
}