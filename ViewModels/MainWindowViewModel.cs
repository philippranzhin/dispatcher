namespace DispatcherDesktop.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using Navigation;
    using Prism.Mvvm;
    using Prism.Regions;

    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        private readonly IRegionsProvider regionsProvider;

        private ObservableCollection<NavigableRegion> regions;

        private NavigableRegion selectedRegion;

        public MainWindowViewModel(IRegionManager regionManager, IRegionsProvider regionsProvider)
        {
            this.regionManager = regionManager;
            this.regionsProvider = regionsProvider;


            this.regions = new ObservableCollection<NavigableRegion>(this.regionsProvider.Regions.Values.Where((r) => r.AvailableForUser));

            this.selectedRegion = this.regionsProvider.SelectedRegion;
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
