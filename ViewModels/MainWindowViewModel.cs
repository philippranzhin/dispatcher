namespace DispatcherDesktop.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using DispatcherDesktop.Device;
    using DispatcherDesktop.Navigation;
    using DispatcherDesktop.Views;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MainWindowViewModel : BindableBase
    {
        private readonly IDeviceDataProvider deviceDataProvider;

        private readonly IRegionManager regionManager;

        private readonly IRegionsProvider regionsProvider;

        private bool surveyEnabled;

        private ObservableCollection<NavigableRegion> regions;

        private NavigableRegion selectedRegion;

        public MainWindowViewModel(IDeviceDataProvider deviceDataProvider, IRegionManager regionManager, IRegionsProvider regionsProvider)
        {
            this.deviceDataProvider = deviceDataProvider;
            this.regionManager = regionManager;
            this.regionsProvider = regionsProvider;

            this.surveyEnabled = deviceDataProvider.SurveyStarted;

            this.regions = new ObservableCollection<NavigableRegion>(this.regionsProvider.Regions.Values.Where((r) => r.AvailableForUser));

            this.selectedRegion = this.regionsProvider.SelectedRegion;
        }

        public bool SurveyEnabled
        {
            get => this.surveyEnabled;
            set
            {
                this.deviceDataProvider.SurveyStarted = value;
                this.SetProperty(ref this.surveyEnabled, value);
            }
        }

        public ObservableCollection<NavigableRegion> Regions
        {
            get => this.regions;
            set => this.SetProperty(ref this.regions, value);
        }

        public NavigableRegion SelectedRegion
        {
            get => this.selectedRegion;
            set
            {
                if (this.selectedRegion == value)
                {
                    return;
                }

                this.SetProperty(ref this.selectedRegion, value);

                this.Navigate(value);
            }
        }

        private void Navigate(NavigableRegion to)
        {
            this.regionManager.Regions[this.regionsProvider.MainRegion.Id].RequestNavigate(to.ViewId);
        }
    }
}
