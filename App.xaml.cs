using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism;


namespace DispatcherDesktop
{
    using DispatcherDesktop.Views;

    using DispatcherDesktop.Modbus;
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
            containerRegistry.RegisterSingleton<IDevicesConfigurationProvider, HardcodedDevices>();
            containerRegistry.RegisterSingleton<IDeviceDataProvider, DeviceDataProvider>();
            containerRegistry.RegisterSingleton<IDeviceDataReader, DeviceDataReader>();
        }

        protected override Window CreateShell()
        {
            return this.Container.Resolve<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var regionManager = this.Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ContentRegion", typeof(DeviceWall));
            regionManager.RegisterViewWithRegion("DeviceDetailsRegion", typeof(DeviceDetail));
        }
    }
}
