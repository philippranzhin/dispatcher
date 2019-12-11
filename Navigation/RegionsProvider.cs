namespace DispatcherDesktop.Navigation
{
    using System.Collections.Generic;

    using Views;

    public class RegionsProvider : IRegionsProvider
    {
        public RegionsProvider()
        {
            this.MainRegion = new NavigableRegion(RegionNames.Main, typeof(MainWindow));
            this.SelectedRegion = new NavigableRegion(RegionNames.DeviceWall, typeof(DeviceWall), "панель устройств");

            this.Regions = new Dictionary<string, NavigableRegion>()
                               {
                                   {
                                       RegionNames.Main, this.MainRegion
                                   },
                                   {
                                       RegionNames.AddRegister,
                                       new NavigableRegion(RegionNames.AddRegister, typeof(AddRegister))
                                   },
                                   {
                                       RegionNames.Log,
                                       new NavigableRegion(RegionNames.Log, typeof(Log))
                                   },
                                   {
                                       RegionNames.WriteRegisterValue,
                                       new NavigableRegion(RegionNames.WriteRegisterValue, typeof(WriteRegisterValue))
                                   },
                                   {
                                       RegionNames.Settings,
                                       new NavigableRegion(
                                           RegionNames.Settings,
                                           typeof(Settings),
                                           "настройки")
                                   },
                                   {
                                       RegionNames.DeviceWall, this.SelectedRegion
                                   },
                                   {
                                       RegionNames.DeviceDetail,
                                       new NavigableRegion(
                                           RegionNames.DeviceDetail,
                                           typeof(DeviceDetail))
                                   },
                               };
        }

        public Dictionary<string, NavigableRegion> Regions { get; }

        public NavigableRegion MainRegion { get; }

        public NavigableRegion SelectedRegion { get; }
    }
}
