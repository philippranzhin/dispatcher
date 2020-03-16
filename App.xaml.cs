using System.Windows.Threading;

namespace DispatcherDesktop
{
    using System.Diagnostics;
    using System.Windows;
    using Domain;
    using Domain.Configuration;
    using Domain.Data.Device;
    using Domain.Data.Storage;
    using Domain.Data.Survey;
    using Domain.Logger;
    using Domain.Web;
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
            containerRegistry.RegisterSingleton<ISurveySettingsProvider, SurveySettingsProvider>();
            containerRegistry.RegisterSingleton<IDevicesConfigurationProvider, DevicesConfigurationProvider>();
            containerRegistry.RegisterSingleton<ISurveyService, SurveyService>();
            containerRegistry.RegisterSingleton<IDeviceIoDriver, FakeIoDriver>();
            containerRegistry.RegisterSingleton<IStorage, InMemoryStorage>();
            containerRegistry.RegisterSingleton<IRegionsProvider, RegionsProvider>();
            containerRegistry.RegisterSingleton<ILogger, Logger>();
            containerRegistry.RegisterSingleton<IDataTransferService, DataTransferService>();
            containerRegistry.RegisterSingleton<IDataManager, DataManager>();

            this.Exit += (s, e) => this.HandleExit();
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
        
        private void HandleExit()
        {
            var dataManager = this.Container.Resolve<IDataManager>();
            
            dataManager.Dispose();
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
