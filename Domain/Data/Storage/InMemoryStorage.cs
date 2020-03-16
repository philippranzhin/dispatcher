namespace DispatcherDesktop.Domain.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using Models;

    public class InMemoryStorage : IStorage
    {
        private readonly object storageSync = new object();

        private readonly Dictionary<RegisterId, List<RegisterDataSlice>> storedData = new Dictionary<RegisterId, List<RegisterDataSlice>>();

        public event EventHandler<RegisterDataSlice> Saved;

        public void Save(RegisterDataSlice dataSlice)
        {
            lock (this.storageSync)
            {
                if (this.storedData.Count > 10000)
                {
                    this.storedData.Clear();
                }

                if (this.storedData.TryGetValue(dataSlice.Id, out var value))
                {
                    value.Add(dataSlice);
                }
                else
                {
                    this.storedData.Add(dataSlice.Id, new List<RegisterDataSlice>() { dataSlice });
                }

                this.Saved?.Invoke(this, dataSlice);
            }
        }

        public List<RegisterDataSlice> Get(RegisterId id)
        {
            lock (this.storageSync)
            {
                return this.storedData.TryGetValue(id, out var records) ? records : new List<RegisterDataSlice>();
            }
        }

        public bool First(Func<RegisterDataSlice, bool> filter, out RegisterDataSlice dataSlice)
        {
            throw new NotImplementedException();
        }

        public List<RegisterDataSlice> Where(Func<RegisterDataSlice, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
