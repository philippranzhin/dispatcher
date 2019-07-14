using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DispatcherDesktop.ViewModels
{
    using DispatcherDesktop.Models;

    using Prism.Mvvm;

    public class AddRegisterViewModel : BindableBase
    {
        private DeviceDescription device;

        private RegisterDescription registerDescription;

        private bool isFloat;

        public AddRegisterViewModel()
        {
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

        public DeviceDescription Device
        {
            get => this.device;
            set => this.SetProperty(ref this.device, value);
        }

        public string Name
        {
            get => this.registerDescription.Name;
            set
            {
                this.registerDescription.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public string Description
        {
            get => this.registerDescription.Description;
            set
            {
                this.registerDescription.Description = value;
                this.RaisePropertyChanged();
            }
        }

        public uint? IntegerAddress
        {
            get => this.registerDescription.IntegerAddress == 0 
                       ? (uint?)null 
                       : this.registerDescription.IntegerAddress;
            set
            {
                this.registerDescription.IntegerAddress = value ?? 0;
                this.RaisePropertyChanged();
            }
        }

        public uint? FloatAddress
        {
            get => this.registerDescription.FloatAddress;
            set
            {
                this.registerDescription.FloatAddress = value;
                this.RaisePropertyChanged();
            }
        }

        public string Postfix
        {
            get => this.registerDescription.Postfix;
            set
            {
                this.registerDescription.Postfix = value;
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
            set => this.SetProperty(ref this.registerDescription, value);
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(this.Name)
                               && !string.IsNullOrWhiteSpace(this.Description)
                               && (!this.IsFloat || this.FloatAddress != null);
    }
}
