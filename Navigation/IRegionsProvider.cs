namespace DispatcherDesktop.Navigation
{
    using System.Collections.Generic;

    public interface IRegionsProvider
    {
        Dictionary<string ,NavigableRegion> Regions { get; }

        NavigableRegion MainRegion { get; }

        NavigableRegion SelectedRegion { get; }
    }
}
