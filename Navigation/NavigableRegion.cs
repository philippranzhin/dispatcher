namespace DispatcherDesktop.Navigation
{
    using System;

    public class NavigableRegion
    {
        public NavigableRegion(string id, Type type, string name)
        {
            this.Name = name;
            this.Type = type;
            this.Id = id;
            this.AvailableForUser = true;
        }

        public NavigableRegion(string id, Type type)
        {
            this.Name = string.Empty;
            this.Type = type;
            this.Id = id;
            this.AvailableForUser = false;
        }

        public string ViewId => this.Type.FullName;

        public string Name { get; }

        public Type Type { get; }

        public string Id { get; }

        public bool AvailableForUser { get; }
    }
}
