namespace DispatcherDesktop
{
    using System.Windows;

    using DispatcherDesktop.Configuration;
    using DispatcherDesktop.Device;
    using DispatcherDesktop.Device.Configuration;
    using DispatcherDesktop.Device.Data;
    using DispatcherDesktop.Device.Survey;
    using DispatcherDesktop.Navigation;
    using DispatcherDesktop.Views;

    using Prism.Ioc;
    using Prism.Regions;
    using Prism.Unity;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ISettingsProvider, SettingsProvider>();
            containerRegistry.RegisterSingleton<IDevicesConfigurationProvider, DevicesConfigurationProvider>();
            containerRegistry.RegisterSingleton<ISurveyService, SurveyService>();
            containerRegistry.RegisterSingleton<IDeviceDataReader, DeviceDataReader>();
            containerRegistry.RegisterSingleton<IStorage, InMemoryStorage>();
            containerRegistry.RegisterSingleton<IRegionsProvider, RegionsProvider>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var regionManager = this.Container.Resolve<IRegionManager>();

            var regionsProvider = this.Container.Resolve<IRegionsProvider>();

            foreach (var regionsProviderRegion in regionsProvider.Regions)
            {
                this.Register(regionsProviderRegion.Value);
            }

            regionManager.Regions[RegionNames.Main].RequestNavigate(regionsProvider.SelectedRegion.ViewId);
        }

        private void Register(NavigableRegion region)
        {
            var regionManager = this.Container.Resolve<IRegionManager>();
            var mainRegion = regionManager.Regions[RegionNames.Main];

            if (mainRegion.Name == region.Id )
            {
                return;
            }

            regionManager.RegisterViewWithRegion(region.Id, () =>
                this.Container.Resolve(region.Type));

            object view = this.Container.Resolve(region.Type);
            
            mainRegion.Add(view);
        }
    }
}
