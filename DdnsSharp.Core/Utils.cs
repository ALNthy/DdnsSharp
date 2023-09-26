using System.Net.NetworkInformation;
using System.Net.Sockets;
using DdnsSharp.Core.Model;
using DdnsSharp.Model;
using DdnsSharp.Core.DdnsClient;

namespace DdnsSharp.Core
{
    public static class Utils
    {
        public static NetworkModel GetNetworkIp()
        {
            NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

            NetworkModel net = new NetworkModel() { Ipv4 = new(), Ipv6 = new() };

            foreach (NetworkInterface networkInterface in networkInterfaces)
            {

                if (networkInterface.OperationalStatus == OperationalStatus.Up &&
                    (networkInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                     networkInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                {
                    Dictionary<string, List<string>> networkv4 = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> networkv6 = new Dictionary<string, List<string>>();
                    List<string> v4 = new List<string>();
                    List<string> v6 = new List<string>();
                    foreach (UnicastIPAddressInformation ipInfo in networkInterface.GetIPProperties().UnicastAddresses)
                    {

                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetworkV6)
                        {
                            v6.Add(ipInfo.Address.ToString());
                        }
                        if (ipInfo.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            v4.Add(ipInfo.Address.ToString());
                        }
                    }
                    networkv4.Add(networkInterface.Name, v4);
                    networkv6.Add(networkInterface.Name, v6);
                    net.Ipv4.Add(networkv4);
                    net.Ipv6.Add(networkv6);
                }
            }
            return net;
        }

        public static IEnumerable<NetinterfaceData> V4NetinterfaceDatas()
        {
            var v4 = GetNetworkIp().Ipv4;
            foreach (var i in v4)
            {
                foreach (var ii in i)
                {
                    foreach (var iii in ii.Value)
                    {
                        yield return new NetinterfaceData() { Name = $"{ii.Key}({iii})", Netinterface = new(ii.Key, (byte)ii.Value.IndexOf(iii)) };
                    }
                }
            }
        }

        public static IEnumerable<NetinterfaceData> V6NetinterfaceDatas()
        {
            var v6 = GetNetworkIp().Ipv6;
            foreach (var i in v6)
            {
                foreach (var ii in i)
                {
                    foreach (var iii in ii.Value)
                    {
                        yield return new NetinterfaceData() { Name = $"{ii.Key}({iii})", Netinterface = new(ii.Key, (byte)ii.Value.IndexOf(iii)) };
                    }
                }
            }
        }

        public static string GetIPV6(Netinterface netinterface)
        {
            var net = GetNetworkIp();
            string ip = net.Ipv6.Find(x => x.ContainsKey(netinterface.Name))[netinterface.Name][netinterface.Index];
            return ip;
        }
        public static string GetIPV4(Netinterface netinterface)
        {
            var net = GetNetworkIp();
            string ip = net.Ipv4.Find(x =>x.ContainsKey(netinterface.Name))[netinterface.Name][netinterface.Index];
            return ip;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ddnsConfig"></param>
        /// <returns></returns>
        public static (string IPV4, string IPV6) GetIP(DdnsConfig ddnsConfig)
        {
            NetworkModel net = GetNetworkIp();
            string ipv4 = net.Ipv4.Find(x => x.ContainsKey(ddnsConfig.IPV4.Netinterface.Name))[ddnsConfig.IPV4.Netinterface.Name][ddnsConfig.IPV4.Netinterface.Index];
            string ipv6 = net.Ipv6.Find(x => x.ContainsKey(ddnsConfig.IPV6.Netinterface.Name))[ddnsConfig.IPV6.Netinterface.Name][ddnsConfig.IPV6.Netinterface.Index];
            return (ipv4, ipv6);
        }
    }
}
