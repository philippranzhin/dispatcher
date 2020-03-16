using System.Linq;
using DispatcherDesktop.Models;

namespace DispatcherDesktop.ViewModels
{
    using System.Windows.Input;
    using Domain.Data.Models;
    using Domain.Data.Storage;
    using Domain.Data.Survey;
    using Domain.Models;
    using Infrastructure.ViewContext;
    using Prism.Commands;
    using Prism.Mvvm;

    class WriteRegisterValueViewModel : BindableBase
    {
        private readonly IStorage storage;
        private readonly ISurveyService surveyService;

        private SubViewDialogContext<RegisterReference> context;
        private double? newValue;
        private double? oldValue;
        private volatile bool loading;
        private bool canSave;
        private RegisterDescription register;
        private DeviceDescription device;

        public WriteRegisterValueViewModel(IStorage storage, ISurveyService surveyService)
        {
            this.storage = storage;
            this.surveyService = surveyService;
        }

        public SubViewDialogContext<RegisterReference> Context
        {
            get => this.context;
            set
            {
                this.SetProperty(ref this.context, value);
                this.context.OnStart += this.HandleStart;
            } 
        }

        private void HandleStart(object sender, RegisterReference e)
        {
            this.OldValue = this.storage.Get(new RegisterId(e.Device.Id, e.Register)).LastOrDefault()?.Value;
            this.Register = e.Register;
            this.device = e.Device;
        }

        public RegisterDescription Register
        {
            get => this.register;
            set => this.SetProperty(ref this.register, value);
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
                if (this.NewValue == null || this.Register.WriteAddress == null)
                {
                    return;
                }

                this.Loading = true;

                var registerWriteData = new RegisterWriteRequest(
                    new RegisterId(this.device.Id, this.Register),
                    this.Register.IntegerAddress,
                    this.Register.FloatAddress,
                    (uint)this.Register.WriteAddress,
                    (double) this.NewValue);

                this.surveyService.ScheduleWriteOperation(registerWriteData, this.OnFinish);
            });

        public ICommand CancelCommand => new DelegateCommand(
            () =>
            {
                this.Loading = false;
                this.NewValue = null;

                this.context.Cancel();
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

        private void OnFinish(bool success)
        {
            if (!this.Loading)
            {
                return;
            }

            this.Loading = false;
            this.NewValue = null;
            this.context.Finish();
        }
    }
}
