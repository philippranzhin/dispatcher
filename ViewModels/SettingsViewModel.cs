namespace DispatcherDesktop.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Domain;
    using Domain.Configuration;
    using Domain.Data.Survey;
    using Domain.Models;
    using Domain.Web;
    using Prism.Commands;
    using Prism.Mvvm;
    using Properties;

    public class SettingsViewModel : BindableBase
    {
        private bool surveyEnabled;

        private readonly ISurveyService surveyService;

        private readonly ISurveySettingsProvider surveySettingsProvider;

        private readonly IDevicesConfigurationProvider configurationProvider;

        private readonly IDataManager dataManager;
        
        private string portName;

        private int surveyPeriod;
        private string serverUrl;
        private string userName;
        private string password;
        private ObservableCollection<DeviceMapper> mappers;
        private uint? mapperSource;
        private uint? mapperRecipient;

        public SettingsViewModel(ISurveyService surveyService, ISurveySettingsProvider surveySettingsProvider, IDataManager dataManager, IDevicesConfigurationProvider configurationProvider)
        {
            this.surveyService = surveyService;
            this.surveySettingsProvider = surveySettingsProvider;
            this.dataManager = dataManager;
            this.configurationProvider = configurationProvider;

            surveyService.SurveyStarted = surveySettingsProvider.SurveyEnabled;
			this.surveyEnabled = surveySettingsProvider.SurveyEnabled;

            this.surveyPeriod = surveySettingsProvider.SurveyPeriodSeconds;
            this.portName = surveySettingsProvider.ConnectionString;

            this.ServerUrl = Settings.Default.SeverURL;
            this.UserName = Settings.Default.UserName;
            this.Password = Settings.Default.Password;

            this.mappers = new ObservableCollection<DeviceMapper>(this.configurationProvider.Mappers);
            
            this.SetWebTransfer();
        }

        public ObservableCollection<DeviceMapper> Mappers
        {
            get => this.mappers;
            set => this.SetProperty(ref this.mappers, value);
        }

        public uint? MapperSource
        {
            get => this.mapperSource;
            set
            { 
                this.SetProperty(ref this.mapperSource, value);
                this.RaisePropertyChanged(nameof(this.CanAddMapper));
            }
        }
        
        public uint? MapperRecipient
        {
            get => this.mapperRecipient;
            set
            {
                this.SetProperty(ref this.mapperRecipient, value);
                this.RaisePropertyChanged(nameof(this.CanAddMapper));
            }
        }

        public bool CanAddMapper => this.MapperSource != null && this.MapperRecipient != null;
        
        public bool SurveyEnabled
        {
            get => this.surveyEnabled;
            set => this.SetProperty(ref this.surveyEnabled, value);
        }

        public string PortName
        {
            get => this.portName;
            set => this.SetProperty(ref this.portName, value);
        }

        public int SurveyPeriod
        {
            get => this.surveyPeriod;
            set => this.SetProperty(ref this.surveyPeriod, value);
        }

        public string ServerUrl
        {
            get => this.serverUrl;
            set => this.SetProperty(ref this.serverUrl, value);
        }

        public string UserName
        {
            get => this.userName;
            set => this.SetProperty(ref this.userName, value);
        }

        public string Password
        {
            get => this.password;
            set => this.SetProperty(ref this.password, value);
        }

        public ICommand SaveConnectionSettingsCommand => new DelegateCommand(this.SaveConnectionSettings);

        public ICommand AddMapperCommand => new DelegateCommand(this.AddMapper);
        
        public ICommand RemoveMapperCommand => new DelegateCommand<DeviceMapper>(this.RemoveMapper);

        private void RemoveMapper(DeviceMapper mapper)
        {
            this.configurationProvider.RemoveMapper(mapper);
            
            this.Mappers = new ObservableCollection<DeviceMapper>(this.configurationProvider.Mappers);
        }

        private void AddMapper()
        {
            var source = this.MapperSource;
            var recipient = this.MapperRecipient;
            
            if (source != null && recipient != null)
            {
                this.configurationProvider.AddMapper(
                    new DeviceMapper()
                    {
                        Source = (uint) source, 
                        Recipient = (uint) recipient
                    });
            }
            
            this.Mappers = new ObservableCollection<DeviceMapper>(this.configurationProvider.Mappers);
        }

        private void SaveConnectionSettings()
        {
			this.surveyService.SurveyStarted = this.SurveyEnabled;

            this.surveySettingsProvider.SurveyEnabled = this.SurveyEnabled;
			this.surveySettingsProvider.SurveyPeriodSeconds = this.SurveyPeriod;
            this.surveySettingsProvider.ConnectionString = this.PortName;

            Settings.Default.SeverURL = this.ServerUrl;
            Settings.Default.UserName = this.UserName;
            Settings.Default.Password = this.Password;

            Settings.Default.Save();

            this.SetWebTransfer();
        }

        private void SetWebTransfer()
        {
            if (this.surveyService.SurveyStarted)
            {
                if (string.IsNullOrWhiteSpace(this.ServerUrl) 
                    || string.IsNullOrWhiteSpace(this.UserName) 
                    || string.IsNullOrWhiteSpace(this.Password))
                {
                    return;
                }
                
                Task.Run(async () => await this.dataManager.Start(new ConnectInfo(new Uri(this.ServerUrl), this.UserName, this.Password)));
            }
            else
            {
                Task.Run(async () => await this.dataManager.Stop());
            }
        }
    }
}
