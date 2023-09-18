namespace DdnsSharp.Model
{
    public class DdnsConfig
    {
        public Guid Guid { get; set; }
        public string Name { get; set; }
        public ServiceType ServiceName { get; set; }
        public string Id { get; set; }
        public string Key { get; set; }
        public NetworkConfig IPV4 { get; set; }
        public NetworkConfig IPV6 { get; set; }
    }
}