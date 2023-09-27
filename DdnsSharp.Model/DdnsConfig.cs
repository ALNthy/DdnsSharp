namespace DdnsSharp.Model
{
    public class DdnsConfig
    {
        public Guid Guid { get; set; }
        public bool Enable { get; set; } = true;
        public string Name { get; set; }
        public ServiceType ServiceName { get; set; } = ServiceType.DnsPod;
        public string Id { get; set; }
        public string Key { get; set; }
        public NetworkConfig IPV4 { get; set; }
        public NetworkConfig IPV6 { get; set; }
        public ulong? Ttl { get; set; }
    }
}