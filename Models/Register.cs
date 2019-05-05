namespace DispatcherDesktop.Models
{
    public class Register
    {
        public Register(RegisterDescription description)
        {
            this.Description = description;
        }

        public RegisterDescription Description { get; }

        public RegisterData Data { get; set; }
    }
}
