namespace DispatcherDesktop.Domain.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Filter = System.Func<Models.RegisterDataSlice, bool>;

    public interface IStorage
    {
        event EventHandler<RegisterDataSlice> Saved;

        void Save(RegisterDataSlice dataSlice);

        List<RegisterDataSlice> Get(RegisterId id);

        bool First(Filter filter, out RegisterDataSlice dataSlice);

        List<RegisterDataSlice> Where(Filter filter);
    }
}
