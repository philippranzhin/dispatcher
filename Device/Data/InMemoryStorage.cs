namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Collections.Generic;

    using Models;

    public class InMemoryStorage : IStorage
    {
        private readonly object storageSync = new object();

        private readonly Dictionary<RegisterId, List<RegisterData>> storedData = new Dictionary<RegisterId, List<RegisterData>>();

        public event EventHandler<RegisterData> Saved;

        public void Save(RegisterData data)
        {
            lock (this.storageSync)
            {
                if (this.storedData.Count > 10000)
                {
                    this.storedData.Clear();
                }

                if (this.storedData.TryGetValue(data.Id, out var value))
                {
                    value.Add(data);
                }
                else
                {
                    this.storedData.Add(data.Id, new List<RegisterData>() { data });
                }

                this.Saved?.Invoke(this, data);
            }
        }

        public List<RegisterData> Get(RegisterId id)
        {
            lock (this.storageSync)
            {
                return this.storedData.TryGetValue(id, out var records) ? records : new List<RegisterData>();
            }
        }

        public bool First(Func<RegisterData, bool> filter, out RegisterData data)
        {
            throw new NotImplementedException();
        }

        public List<RegisterData> Where(Func<RegisterData, bool> filter)
        {
            throw new NotImplementedException();
        }
    }
}
