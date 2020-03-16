namespace DispatcherDesktop.Models
{
    using Domain.Data.Models;
    using Domain.Models;
    using Prism.Mvvm;

    public class Register : BindableBase
    {
        private RegisterDataSlice dataSlice;

        public Register(RegisterDescription description)
        {
            this.Description = description;

            this.CanWrite = description.WriteAddress != null;
        }

        public RegisterDescription Description { get; }

        public RegisterDataSlice DataSlice
        {
            get => this.dataSlice;
            set => this.SetProperty(ref this.dataSlice, value);
        }

        public bool CanWrite { get; }

        public bool HasValue => this.DataSlice != null;
    }
}
