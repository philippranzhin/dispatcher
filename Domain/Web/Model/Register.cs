namespace DispatcherDesktop.Domain.Web.Model
{
    public struct Register
    {
        public Register(string address, string value)
        {
            this.Address = address;
            this.Value = value;
        }

        public string Address { get; }
        
        public string Value { get; }
    }
}