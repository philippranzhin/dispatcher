namespace DispatcherDesktop.Navigation
{
    using DispatcherDesktop.Views;

    public static class RegionNames
    {
        public const string Main = "MainRegion";

        public const string DeviceWall = "DeviceWallRegion";

        public const string DeviceDetail = "DeviceDetailRegion";

        public const string Settings = "SettingsRegion";

        public static string RegionNameToView(string name)
        {
            switch (name)
            {
                case Main: 
                    return typeof(MainWindow).FullName;
                case DeviceWall:
                    return typeof(DeviceWall).FullName;
                case DeviceDetail:
                    return typeof(DeviceDetail).FullName;
                case Settings:
                    return typeof(Settings).FullName;
                default:
                    return string.Empty;
            }
        }
    }
}
