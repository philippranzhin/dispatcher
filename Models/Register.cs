namespace DispatcherDesktop.Models
{
    public class Register
    {
        public Register(RegisterDescription description)
        {
            this.Description = description;

            this.CanWrite = description.WriteAddress != null;
        }

        public RegisterDescription Description { get; }

        public RegisterData Data { get; set; }

        public bool CanWrite { get; }

        public bool HasValue => this.Data != null;
    }
}
