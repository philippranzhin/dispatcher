namespace DispatcherDesktop.ViewModels
{
    using System.Windows.Input;
    using Device.Configuration;
    using Infrastructure.Models;
    using Infrastructure.ViewContext;
    using Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class AddRegisterViewModel : BindableBase
    {
        private readonly IDevicesConfigurationProvider devicesConfiguration;

        private RegisterDescription registerDescription;

        private bool isFloat;
        private EditRegisterContext context;
        private EditRegisterMode mode;

        public AddRegisterViewModel(IDevicesConfigurationProvider devicesConfiguration)
        {
            this.devicesConfiguration = devicesConfiguration;
            this.PropertyChanged += (s, e) =>
                {
                    var canSaveName = nameof(this.CanSave);

                    if (e.PropertyName != canSaveName)
                    {
                        this.RaisePropertyChanged(nameof(this.CanSave));
                    }
                };

            this.registerDescription = new RegisterDescription();
        }

        public EditRegisterContext Context
        {
            get => this.context;
            set
            {
                this.context = value;

                this.context.OnStart += this.Start;
            }
        }

        public string Name
        {
            get => this.RegisterDescription.Name;
            set
            {
                this.RegisterDescription.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public string Description
        {
            get => this.RegisterDescription.Description;
            set
            {
                this.RegisterDescription.Description = value;
                this.RaisePropertyChanged();
            }
        }

        public uint? IntegerAddress
        {
            get => this.registerDescription.IntegerAddress == 0 
                       ? (uint?)null 
                       : this.RegisterDescription.IntegerAddress;
            set
            {
                this.RegisterDescription.IntegerAddress = value ?? 0;
                this.RaisePropertyChanged();
            }
        }

        public uint? FloatAddress
        {
            get => this.RegisterDescription.FloatAddress;
            set
            {
                this.RegisterDescription.FloatAddress = value;
                this.RaisePropertyChanged();
            }
        }

        public uint? WriteAddress
        {
            get => this.RegisterDescription.WriteAddress;
            set
            {
                this.RegisterDescription.WriteAddress = value;
                this.RaisePropertyChanged();
            }
        }

        public string Postfix
        {
            get => this.RegisterDescription.Postfix;
            set
            {
                this.RegisterDescription.Postfix = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsFloat
        {
            get => this.isFloat;
            set => this.SetProperty(ref this.isFloat, value);
        }

        public RegisterDescription RegisterDescription
        {
            get => this.registerDescription;
            set
            {
                this.SetProperty(ref this.registerDescription, value);
                this.RaisePropertyChanged(nameof(this.Description));
                this.RaisePropertyChanged(nameof(this.FloatAddress));
                this.RaisePropertyChanged(nameof(this.IntegerAddress));
                this.RaisePropertyChanged(nameof(this.Name));
                this.RaisePropertyChanged(nameof(this.Postfix));
                this.RaisePropertyChanged(nameof(this.WriteAddress));
            } 
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(this.Name)
                               && !string.IsNullOrWhiteSpace(this.Description)
                               && (!this.IsFloat || this.FloatAddress != null);


        public EditRegisterMode Mode
        {
            get => this.mode;
            set => this.SetProperty(ref this.mode, value);
        }

        public ICommand SaveCommand => new DelegateCommand(
            () =>
            {
                if (this.Mode == EditRegisterMode.Create)
                {
                    this.context.SelectedDevice.Registers.Add(this.RegisterDescription);
                }
                

                this.devicesConfiguration.Save(this.context.SelectedDevice);

                this.Context.Finish();

                this.RegisterDescription = new RegisterDescription();
            });

        public ICommand CancelCommand => new DelegateCommand(
            () =>
            {
                this.RegisterDescription = new RegisterDescription();

                this.Context.Cancel();
            });

        private void Start(object sender, RegisterDescription e)
        {
            if (e == null)
            {
                this.Mode = EditRegisterMode.Create;
                this.RegisterDescription = new RegisterDescription();

                this.IsFloat = false;
            }
            else
            {
                this.Mode = EditRegisterMode.Edit;
                this.RegisterDescription = e;
                this.IsFloat = e.FloatAddress != null;
            }
        }
    }
}
