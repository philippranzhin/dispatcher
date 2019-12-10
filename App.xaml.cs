using System;
using System.Windows.Threading;

namespace DispatcherDesktop
{
    using System.Windows;

    using Configuration;
    using Device;
    using DispatcherDesktop.Device.Configuration;
    using Device.Data;
    using Device.Logger;
    using Device.Survey;
    using Navigation;
    using Views;

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
            containerRegistry.RegisterSingleton<IDeviceIoDriver, DeviceIoDriver>();
            containerRegistry.RegisterSingleton<IStorage, InMemoryStorage>();
            containerRegistry.RegisterSingleton<IRegionsProvider, RegionsProvider>();
            containerRegistry.RegisterSingleton<IUiLogger, UiLogger>();
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

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "\n"+e.Exception.StackTrace, "Exception", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
