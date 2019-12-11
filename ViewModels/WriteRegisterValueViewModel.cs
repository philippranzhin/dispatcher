using System.Linq;
using DispatcherDesktop.Device.Data;
using DispatcherDesktop.Models;

namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Device.Survey;
    using Prism.Commands;
    using Prism.Mvvm;

    class WriteRegisterValueViewModel : BindableBase
    {
        private readonly IStorage storage;
        private readonly ISurveyService surveyService;

        private RegisterReference register;
        private double? newValue;
        private double? oldValue;
        private volatile bool loading;
        private bool canSave;

        public WriteRegisterValueViewModel(IStorage storage, ISurveyService surveyService)
        {
            this.storage = storage;
            this.surveyService = surveyService;
        }

        public RegisterReference Register
        {
            get => this.register;
            set
            {
                this.SetProperty(ref this.register, value);
                this.OldValue = this.storage.Get(new RegisterId(value.DeviceId, value.RegisterDescription)).LastOrDefault()?.Value;
            } 
        }

        public double? NewValue
        {
            get => this.newValue;
            set
            {
                this.SetProperty(ref this.newValue, value);
                this.CanSave = value != null;
            }
        }

        public ICommand WriteCommand => new DelegateCommand(
            () =>
            {
                if (this.NewValue == null || this.register.RegisterDescription.WriteAddress == null)
                {
                    return;
                }

                this.Loading = true;

                var registerWriteData = new RegisterWriteData(
                    new RegisterId(this.register.DeviceId, this.register.RegisterDescription),
                    this.register.RegisterDescription.IntegerAddress,
                    this.register.RegisterDescription.FloatAddress,
                    (uint)this.register.RegisterDescription.WriteAddress,
                    (double) this.NewValue);

                this.surveyService.ScheduleWriteOperation(registerWriteData, this.OnFinish);
            });

        public bool Loading
        {
            get => this.loading;
            set
            {
                this.loading = value;
                this.RaisePropertyChanged();
            }
        }

        public bool CanSave
        {
            get => this.canSave;
            set => this.SetProperty(ref this.canSave, value);
        }

        public double? OldValue
        {
            get => this.oldValue;
            set => this.SetProperty(ref this.oldValue, value);
        }

        public event EventHandler OnOperationFinish;

        private void OnFinish(bool success)
        {
            if (!this.Loading)
            {
                return;
            }

            this.OnOperationFinish?.Invoke(this, EventArgs.Empty);
            this.Loading = false;
            this.NewValue = null;
        }
    }
}
