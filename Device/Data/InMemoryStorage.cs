namespace DispatcherDesktop.Device.Data
{
    using System;
    using System.Collections.Generic;

    using DispatcherDesktop.Models;

    public class InMemoryStorage : IStorage
    {
        private Dictionary<RegisterId, List<RegisterData>> data = new Dictionary<RegisterId, List<RegisterData>>();

        public event EventHandler<RegisterData> Saved;

        public void Save(RegisterData data)
        {
            if (this.data.TryGetValue(data.Id, out var value))
            {
                value.Add(data);
            }
            else
            {
                this.data.Add(data.Id, new List<RegisterData>(){ data });
            }
            
            this.Saved?.Invoke(this, data);
        }

        public List<RegisterData> Get(RegisterId id)
        {
            return this.data.TryGetValue(id, out var records) ? records : new List<RegisterData>();
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
