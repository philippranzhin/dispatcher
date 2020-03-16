namespace DispatcherDesktop.ViewModels
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using Domain.Configuration;
    using Domain.Models;
    using Infrastructure.Models;
    using Infrastructure.ViewContext;
    using Models;
    using Prism.Commands;
    using Prism.Mvvm;

    public class EditRegisterViewModel : BindableBase
    {
        private readonly IDevicesConfigurationProvider devicesConfiguration;

        private RegisterDescription registerDescription;

        private bool isFloat;
        private SubViewDialogContext<RegisterReference> context;
        private EditRegisterMode mode;
        private DeviceDescription device;
        private bool isWritable;

        public EditRegisterViewModel(IDevicesConfigurationProvider devicesConfiguration)
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

        public SubViewDialogContext<RegisterReference> Context
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
            set
            {
                this.SetProperty(ref this.isFloat, value);

                if (!value)
                {
                    this.FloatAddress = null;    
                }
            } 
        }

        public bool IsWritable
        {
            get => this.isWritable;
            set
            {
                this.SetProperty(ref this.isWritable, value);

                if (!value)
                {
                    this.WriteAddress = null;    
                }
            } 
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
                this.RaisePropertyChanged(nameof(this.IsFloat));
                this.RaisePropertyChanged(nameof(this.IsWritable));
            } 
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(this.Name)
                               && !string.IsNullOrWhiteSpace(this.Description)
                               && (!this.IsFloat || this.FloatAddress != null)
                               && (!this.IsWritable || this.WriteAddress != null);


        public EditRegisterMode Mode
        {
            get => this.mode;
            set => this.SetProperty(ref this.mode, value);
        }

        public ICommand SaveCommand => new DelegateCommand(
            () =>
            {
                List<RegisterDescription> registers = this.device.Registers;
                
                if (this.device.Registers.Any(r => r.IntegerAddress == this.RegisterDescription.IntegerAddress))
                {
                    registers = this.device.Registers.Select(r =>
                    {
                        if (r.IntegerAddress == this.RegisterDescription.IntegerAddress)
                        {
                            return this.RegisterDescription;
                        }

                        return r;
                    }).ToList();
                }
                else
                {
                    registers.Add(this.RegisterDescription);   
                }

                this.devicesConfiguration.AddDevice(new DeviceDescription(this.device.Id, this.device.Name, registers));

                this.Context.Finish();

                this.RegisterDescription = new RegisterDescription();
            });

        public ICommand CancelCommand => new DelegateCommand(
            () =>
            {
                this.RegisterDescription = new RegisterDescription();

                this.Context.Cancel();
            });

        private void Start(object sender, RegisterReference e)
        {
            this.device = e.Device;

            if (e.Register == RegisterDescription.Empty)
            {
                this.Mode = EditRegisterMode.Create;
                this.RegisterDescription = new RegisterDescription();
                this.IsFloat = false;
                this.IsWritable = false;
            }
            else
            {
                this.Mode = EditRegisterMode.Edit;
                this.RegisterDescription = e.Register.Clone() as RegisterDescription;
                this.IsFloat = e.Register.FloatAddress != null;
                this.IsWritable = e.Register.WriteAddress != null;
            }
        }
    }
}
