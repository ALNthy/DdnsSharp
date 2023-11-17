namespace DdnsSharp.Model
{
    public class NetworkConfig
    {
        public Guid Guid { get; init; } =Guid.NewGuid();
        public bool Enable { get; set; }
        public GetIPType Type { get; set; }
        public Netinterface Netinterface { get; set; }
        public string Url { get; set; }
        public string Domains { get; set; }

        public string[] GetDomains()
        {
            if (string.IsNullOrEmpty(Domains))
            {
                return Array.Empty<string>();
            }
            else
            {
                return Domains.Split("\n");
            }
        }
    }
}
