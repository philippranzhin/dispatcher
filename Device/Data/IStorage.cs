namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Collections.Generic;

    using Models;

    using Filter = System.Func<Models.RegisterData, bool>;

    public interface IStorage
    {
        event EventHandler<RegisterData> Saved;

        void Save(RegisterData data);

        List<RegisterData> Get(RegisterId id);

        bool First(Filter filter, out RegisterData data);

        List<RegisterData> Where(Filter filter);
    }
}
